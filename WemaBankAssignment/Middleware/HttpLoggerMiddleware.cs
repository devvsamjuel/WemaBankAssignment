using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WemaBankAssignment.Middleware
{
    public class HttpLoggerMiddleware
    {
        RequestDelegate next;
        private readonly ILogger<HttpLoggerMiddleware> logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public HttpLoggerMiddleware(RequestDelegate next, ILogger<HttpLoggerMiddleware> _log)
        {
            this.next = next;
            logger = _log;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await HandleLogging(context);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Error in HttpLoggerMiddleware: {ex}");
            }

        }

        private async Task HandleLogging(HttpContext context)
        {
            #region Request

            //Request handling comes here

            // create a new log object
            var log = new RequestResponseLog
            {
                Endpoint = context.Request.Path,
                HttpMethod = context.Request.Method,
                QueryString = context.Request.QueryString.ToString(),
                Host = context.Request.Host.ToString(),
                Schema = context.Request.Scheme
            };

            // check if the Request is a POST call 
            // since we need to read from the body

            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);
                var body = ReadStreamInChunks(requestStream);
                context.Request.Body.Position = 0;
                log.RequestBody = body;

            }

            //if (context.Request.Method == "POST")
            //{
            //    //context.Request.EnableBuffering();
            //    //var body = await new StreamReader(context.Request.Body)
            //    //                                    .ReadToEndAsync();
            //    context.Request.Body.Position = 0;
            //    log.Body = body;
            //}

            log.RequestTime = DateTime.Now;
            #endregion

            await next.Invoke(context);

            #region Response
            using (Stream originalResponse = context.Response.Body)
            {
                try
                {
                    using (var memStream = new MemoryStream())
                    {
                        context.Response.Body = memStream;
                        // All the Request processing as described above 
                        // happens from here.
                        // Response handling starts from here
                        // set the pointer to the beginning of the 
                        // memory stream to read

                        //memStream.Position = 0;

                        // read the memory stream till the end


                        memStream.Seek(0, SeekOrigin.Begin);
                        //context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var response = await new StreamReader(memStream)
                                                                .ReadToEndAsync();

                        // write the response to the log object
                        log.Response = response;
                        log.ResponseCode = context.Response.StatusCode.ToString();
                        log.IsSuccessStatusCode =
                              context.Response.StatusCode == 200 ||
                              context.Response.StatusCode == 201;
                        log.ResponseTime = DateTime.Now;

                        // add the log object to the logger stream 
                        // via the Repo instance injected
                        logger.LogInformation(JsonConvert.SerializeObject(log));

                        // since we have read till the end of the stream, 
                        // reset it onto the first position

                        //memStream.Position = 0;

                        // now copy the content of the temporary memory 
                        // stream we have passed to the actual response body 
                        // which will carry the response out.
                        memStream.Seek(0, SeekOrigin.Begin);
                        //context.Response.Body.Seek(0, SeekOrigin.Begin);
                        await memStream.CopyToAsync(originalResponse);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error in HttpLoggerMiddleware: {ex}");
                }
                finally
                {
                    // assign the response body to the actual context
                    context.Response.Body = originalResponse;
                }
            }
            #endregion
        }
        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }
    }

    public class RequestResponseLog
    {
        public string Endpoint { get; set; }
        public string Host { get; set; }
        public string Schema { get; set; }
        public string QueryString { get; set; }
        public string HttpMethod { get; set; }
        public string RequestBody { get; set; }
        public string Response { get; set; }
        public string ResponseCode { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
