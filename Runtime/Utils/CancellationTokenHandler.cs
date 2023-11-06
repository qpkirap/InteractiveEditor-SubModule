using System.Threading;

namespace Module.InteractiveEditor.Runtime
{
    public class CancellationTokenHandler
    {
        private CancellationTokenSource tokenSource = new ();

        private bool canceled;

        public CancellationToken Token
        {
            get
            {
                if (canceled)
                {
                    CreateTokenSource();
                }

                return tokenSource.Token;
            }
        }

        public void CancelOperation()
        {
            if (canceled)
            {
                return;
            }
            
            canceled = true;
            
            tokenSource.Cancel(false);
            tokenSource = default;
        }
        
        private void CreateTokenSource()
        {
            canceled = false;
            
            tokenSource = new CancellationTokenSource();
        }
    }
}