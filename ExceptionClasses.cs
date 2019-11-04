using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powell.Mfg.LibraryWinService
{
    public class InvalidPathException : Exception
    {
        public InvalidPathException()
        {
        }

        public InvalidPathException(string message) : base(message)
        {
            
        }
    }

    public class MissingDXFException : Exception
    {
        public MissingDXFException()
        {
        }

        public MissingDXFException(string message)
            : base(message)
        {

        }
    }

    public class ZeroLengthDXFException : Exception
    {
        public ZeroLengthDXFException()
        {
        }

        public ZeroLengthDXFException(string message)
            : base(message)
        {

        }
    }

    public class DxfLayerException : Exception
    {
        public DxfLayerException()
        {
        }

        public DxfLayerException(string message)
            : base(message)
        {

        }
    }

    public class DestinationPathException : Exception
    {
        public DestinationPathException()
        {
        }

        public DestinationPathException(string message)
            : base(message)
        {

        }
    }

    public class AgileConfigurationException : Exception
    {
        public AgileConfigurationException()
        {
        }

        public AgileConfigurationException(string message)
            : base(message)
        {

        }
    }

    public class PPIFileException : Exception
    {
        public PPIFileException()
        {
        }

        public PPIFileException(string message)
            : base(message)
        {

        }
    }

    public class InvalidOptimationBatFileException : Exception
    {
        public InvalidOptimationBatFileException()
        {
        }

        public InvalidOptimationBatFileException(string message)
            : base(message)
        {

        }
    }

    public class OptimationTimeoutException : Exception
    {
        public OptimationTimeoutException()
        {
        }

        public OptimationTimeoutException(string message)
            : base(message)
        {

        }
    }

    public class AutoSyncException : Exception
    {
        public AutoSyncException()
        {
        }

        public AutoSyncException(string message)
            : base(message)
        {

        }
    }

    public class InvalidSyncException : Exception
    {
        public InvalidSyncException()
        {
        }

        public InvalidSyncException(string message)
            : base(message)
        {

        }
    }

    public class UnsuccessfulSyncException : Exception
    {
        public UnsuccessfulSyncException()
        {
        }

        public UnsuccessfulSyncException(string message)
            : base(message)
        {

        }
    }
    
    public class GEOMappingException : Exception
    {
        public GEOMappingException()
        {
        }

        public GEOMappingException(string message)
            : base(message)
        {

        }
    }

    public class PEFMappingException : Exception
    {
        public PEFMappingException()
        {
        }

        public PEFMappingException(string message)
            : base(message)
        {

        }
    }

    public class LaunchProcessException : Exception
    {
        public LaunchProcessException()
        {
        }

        public LaunchProcessException(string message)
            : base(message)
        {

        }
    }

    public class SyncRequestException : Exception
    {
        public SyncRequestException()
        {
        }

        public SyncRequestException(string message)
            : base(message)
        {

        }
    }

    public class InvalidPartException : Exception
    {
        public InvalidPartException()
        {
        }

        public InvalidPartException(string message)
            : base(message)
        {

        }
    }
}
