// http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html

public class ThreadedJob
{
    private bool m_IsDone = false;
    private object m_Handle = new object();
    private System.Threading.Thread m_Thread = null;
    public bool IsDone
    {
        get {
            bool tmp;
            lock (m_Handle) {
                tmp = m_IsDone;
            }
            return tmp;
        }
        set {
            lock (m_Handle) {
                m_IsDone = value;
            }
        }
    }
    private bool m_IsAborted = false;

    public virtual void Start() {
        m_Thread = new System.Threading.Thread(Run);
        m_Thread.Start();
    }
    public virtual void Abort() {
        if (m_IsAborted || IsDone || m_Thread == null) { return; }
        m_Thread.Abort();
        m_IsAborted = true;
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }

    public virtual bool Update() {
        if (IsDone) {
            OnFinished();
            return true;
        }
        return false;
    }

    private void Run() {
        ThreadFunction();
        IsDone = true;
    }
}
