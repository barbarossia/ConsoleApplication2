using System;
using System.Collections;
using System.Threading;
namespace MapReduce.Parser.UnitTest {
    public sealed class MyThreadPool {
        private class WorkerThread {
            private object _workid;
            private Thread _thread;
            private AutoResetEvent _workAssignedEvent = new AutoResetEvent(false);
            private AutoResetEvent _workCompleteEvent;
            private ThreadPoolCallback _workItem;
            private object _workData;
            private bool _assigned;
            private bool _stop;
            private readonly object _stopLock = new object();
            private bool Stopping
            {
                get
                {
                    object stopLock;
                    Monitor.Enter(stopLock = this._stopLock);
                    bool stop;
                    try {
                        stop = this._stop;
                    } finally {
                        Monitor.Exit(stopLock);
                    }
                    return stop;
                }
            }
            public bool IsAssigned
            {
                get
                {
                    return this._assigned;
                }
            }
            public object ID
            {
                get
                {
                    return this._workid;
                }
            }
            public object Data
            {
                get
                {
                    return this._workData;
                }
            }
            private void _workerStart() {
                try {
                    while(!this.Stopping) {
                        while(!this._workAssignedEvent.WaitOne(5000, false)) {
                            if(this.Stopping) {
                                return;
                            }
                        }
                        try {
                            this._workItem(this._workid, this._workData);
                        } catch {
                        }
                        this._assigned = false;
                        this._workCompleteEvent.Set();
                    }
                } finally {
                    this.SetStopped();
                }
            }
            private void SetStopped() {
                object stopLock;
                Monitor.Enter(stopLock = this._stopLock);
                try {
                    this._stop = false;
                } finally {
                    Monitor.Exit(stopLock);
                }
            }
            public WorkerThread(AutoResetEvent workCompleteEvent) {
                this._workCompleteEvent = workCompleteEvent;
                this._thread = new Thread(new ThreadStart(this._workerStart));
                this._thread.IsBackground = true;
                this._thread.Start();
            }
            public void AssignWork(object id, ThreadPoolCallback item, object data) {
                this._assigned = true;
                this._workid = id;
                this._workItem = item;
                this._workData = data;
                this._workAssignedEvent.Set();
            }
            public void Release() {
                object stopLock;
                Monitor.Enter(stopLock = this._stopLock);
                try {
                    this._stop = true;
                } finally {
                    Monitor.Exit(stopLock);
                }
            }
        }
        private const int WORKITEM_ASSIGNED = 0;
        private const int WORKITEM_QUEUED = 1;
        private int _maxThreads;
        private int _queueLowThreshhold;
        private bool _enabled;
        private ArrayList _assignedThreads = new ArrayList();
        private ArrayList _availableThreads = new ArrayList();
        private Thread _controllerThread;
        private Queue _workQueue = Queue.Synchronized(new Queue());
        private Hashtable _workItemHash = Hashtable.Synchronized(new Hashtable());
        private Hashtable _workDataHash = Hashtable.Synchronized(new Hashtable());
        private AutoResetEvent _workCompleteEvent = new AutoResetEvent(false);
        private AutoResetEvent _workItemQueuedEvent = new AutoResetEvent(false);
        private ManualResetEvent _queueLowEvent = new ManualResetEvent(true);
        private WaitHandle[] _controllerEvents = new WaitHandle[2];
        public int QueueLowThreshhold
        {
            get
            {
                return this._queueLowThreshhold;
            }
            set
            {
                this._queueLowThreshhold = value;
            }
        }
        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                this._enabled = value;
                this._workItemQueuedEvent.Set();
            }
        }
        public int MaxThreads
        {
            get
            {
                return this._maxThreads;
            }
            set
            {
                if(value >= 0) {
                    this._maxThreads = value;
                }
                if(this._enabled) {
                    this._workItemQueuedEvent.Set();
                }
            }
        }
        public int ThreadCount
        {
            get
            {
                return this._availableThreads.Count + this._assignedThreads.Count;
            }
        }
        public int AvailableThreads
        {
            get
            {
                return this._availableThreads.Count;
            }
        }
        public int ItemsInQueue
        {
            get
            {
                return this._workQueue.Count;
            }
        }
        private void _assignWorkers() {
            while(this._workQueue.Count > 0 && this._enabled) {
                MyThreadPool.WorkerThread workerThread = null;
                if(this._availableThreads.Count > 0) {
                    workerThread = (this._availableThreads[0] as MyThreadPool.WorkerThread);
                    this._availableThreads.RemoveAt(0);
                } else {
                    if(this._assignedThreads.Count < this._maxThreads) {
                        workerThread = new MyThreadPool.WorkerThread(this._workCompleteEvent);
                    }
                }
                if(workerThread == null) {
                    break;
                }
                object obj = this._workQueue.Dequeue();
                object data = this._workDataHash[obj];
                ThreadPoolCallback item = (ThreadPoolCallback)this._workItemHash[obj];
                this._assignedThreads.Add(workerThread);
                workerThread.AssignWork(obj, item, data);
                if(this._workQueue.Count < this._queueLowThreshhold) {
                    this._queueLowEvent.Set();
                }
            }
        }
        private void _unassignWorkers() {
            int i = 0;
            while(i < this._assignedThreads.Count) {
                MyThreadPool.WorkerThread workerThread = this._assignedThreads[i] as MyThreadPool.WorkerThread;
                if(!workerThread.IsAssigned) {
                    this._assignedThreads.RemoveAt(i);
                    if(this._workItemHash.ContainsKey(workerThread.ID)) {
                        this._workItemHash.Remove(workerThread.ID);
                    }
                    if(this._workDataHash.ContainsKey(workerThread.ID)) {
                        this._workDataHash.Remove(workerThread.ID);
                    }
                    this._availableThreads.Add(workerThread);
                } else {
                    i++;
                }
            }
        }
        private void _releaseWorkers() {
            while((this.ThreadCount > this._maxThreads || !this._enabled) && this._availableThreads.Count > 0) {
                MyThreadPool.WorkerThread workerThread = this._availableThreads[0] as MyThreadPool.WorkerThread;
                this._availableThreads.RemoveAt(0);
                workerThread.Release();
            }
        }
        private void _resetLowQueueEvent() {
            if(this._workQueue.Count > this._queueLowThreshhold) {
                this._queueLowEvent.Reset();
            }
        }
        private void _controllerStart() {
            while(true) {
                WaitHandle.WaitAny(this._controllerEvents);
                this._resetLowQueueEvent();
                this._unassignWorkers();
                this._releaseWorkers();
                this._assignWorkers();
            }
        }
        public MyThreadPool(int maxThreads, int queueLowThreshhold) {
            this._maxThreads = maxThreads;
            this._queueLowThreshhold = queueLowThreshhold;
            this._controllerEvents[0] = this._workCompleteEvent;
            this._controllerEvents[1] = this._workItemQueuedEvent;
            this._controllerThread = new Thread(new ThreadStart(this._controllerStart));
            this._controllerThread.IsBackground = true;
            this._controllerThread.Start();
        }
        public MyThreadPool(int maxThreads) {
            this._maxThreads = maxThreads;
            this._controllerEvents[0] = this._workCompleteEvent;
            this._controllerEvents[1] = this._workItemQueuedEvent;
            this._controllerThread = new Thread(new ThreadStart(this._controllerStart));
            this._controllerThread.IsBackground = true;
            this._controllerThread.Start();
        }
        public bool IsWorkItemQueued(object workid) {
            return this._workItemHash.ContainsKey(workid);
        }
        public bool QueueWorkItem(object workid, ThreadPoolCallback cb) {
            return this.QueueWorkItem(workid, null, cb);
        }
        public bool QueueWorkItem(object workid, object data, ThreadPoolCallback cb) {
            if(workid == null) {
                throw new ArgumentNullException("workid", "workid can not be null");
            }
            if(cb == null) {
                throw new ArgumentNullException("item", "item can not be null");
            }
            if(!this.IsWorkItemQueued(workid)) {
                this._workItemHash.Add(workid, cb);
                this._workDataHash.Add(workid, data);
                this._workQueue.Enqueue(workid);
                this._workItemQueuedEvent.Set();
                return true;
            }
            return false;
        }
        public void WaitForQueueLow() {
            this._queueLowEvent.WaitOne();
        }
        public bool WaitForQueueLow(int timeout) {
            return this._queueLowEvent.WaitOne(timeout, false);
        }
        public void ClearWorkQueue() {
            this._workQueue.Clear();
            this._workItemHash.Clear();
            this._workDataHash.Clear();
        }
    }
}
