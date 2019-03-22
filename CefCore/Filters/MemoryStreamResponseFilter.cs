using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.CefCore.Filters
{
    public class MemoryStreamResponseFilter : IResponseFilter
    {
        private MemoryStream memoryStream;
        private List<byte> dataOutBuffer = new List<byte>();
        public string UrlMonitorId { get; set; }
        bool IResponseFilter.InitFilter()
        {
            //NOTE: We could initialize this earlier, just one possible use of InitFilter
            memoryStream = new MemoryStream();

            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                var maxWrite = Math.Min(dataOutBuffer.Count, dataOut.Length);

                //Write the maximum portion that fits in dataOut.
                if (maxWrite > 0)
                {
                    dataOut.Write(dataOutBuffer.ToArray(), 0, (int)maxWrite);
                    dataOutWritten += maxWrite;
                }

                //If dataOutBuffer is bigger than dataOut then we'll write the
                // data on the second pass
                if (maxWrite < dataOutBuffer.Count)
                {
                    // Need to write more bytes than will fit in the output buffer. 
                    // Remove the bytes that were written already
                    dataOutBuffer.RemoveRange(0, (int)(maxWrite));

                    return FilterStatus.NeedMoreData;
                }

                //All data was written, so we clear the buffer and return FilterStatus.Done
                dataOutBuffer.Clear();
                return FilterStatus.Done;
            }

            //Copy data to stream
            dataIn.Position = 0;
            dataIn.CopyTo(memoryStream);
            dataIn.Position = 0;

            //We're going to read all of dataIn
            dataInRead = dataIn.Length;

            var dataInBuffer = new byte[(int)dataIn.Length];
            dataIn.Read(dataInBuffer, 0, dataInBuffer.Length);

            //Add all the bytes to the dataOutBuffer
            dataOutBuffer.AddRange(dataInBuffer);

            dataOutWritten = 0;

            //The assumption here is dataIn is smaller than dataOut then this will be the last of the
            //data to read and we can start processing, this is not a production tested assumption
            if (dataIn.Length < dataOut.Length)
            {
                var bytes = dataOutBuffer.ToArray();

                //Clear the buffer and add the processed data, it's possible
                //that with our processing the data has become to large to fit in dataOut
                //So we'll have to write the data in multiple passes in that case
                dataOutBuffer.Clear();
                dataOutBuffer.AddRange(bytes);

                var maxWrite = Math.Min(dataOutBuffer.Count, dataOut.Length);

                //Write the maximum portion that fits in dataOut.
                if (maxWrite > 0)
                {
                    dataOut.Write(dataOutBuffer.ToArray(), 0, (int)maxWrite);
                    dataOutWritten += maxWrite;
                }

                //If dataOutBuffer is bigger than dataOut then we'll write the
                // data on the second pass
                if (maxWrite < dataOutBuffer.Count)
                {
                    // Need to write more bytes than will fit in the output buffer. 
                    // Remove the bytes that were written already
                    dataOutBuffer.RemoveRange(0, (int)(maxWrite));

                    return FilterStatus.NeedMoreData;
                }

                //All data was written, so we clear the buffer and return FilterStatus.Done
                dataOutBuffer.Clear();

                return FilterStatus.Done;
            }
            else
            {
                //We haven't got all of our dataIn yet, so we keep buffering so that when it's finished
                //we can process the buffer, replace some words etc and then write it all out.
                return FilterStatus.NeedMoreData;
            }
        }

        void IDisposable.Dispose()
        {
            memoryStream.Dispose();
            memoryStream = null;
        }

        public byte[] Data
        {
            get
            {
                return memoryStream.ToArray();
            }
        }
    }
}
