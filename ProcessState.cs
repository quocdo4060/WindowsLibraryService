using Powell.Mfg.LibraryWinService.LibraryServiceReference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powell.Mfg.LibraryWinService
{
    public class ProcessState
    {
        public List<QueueRecord> StateQueueRecords;
        public EventLog StateEventLog;
        public List<DxfLayers> StateLayers;
        public ProcessState(List<QueueRecord> lRecords, EventLog lLog, List<DxfLayers> llayers)
        {
            this.StateEventLog = lLog;
            this.StateQueueRecords = lRecords;
            this.StateLayers = llayers;
        }
    }
}
