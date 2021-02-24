using System.ServiceProcess;
using System.Threading;

namespace Listener
{
    public partial class httpListener : ServiceBase
    {
        Listener myObjListener;

        public httpListener()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            myObjListener = new Listener
            {
                prefix = args.Length > 0 ? $"http://{args[0]}" + $":8080/" : $"http://127.0.0.1" + $":8080/"
            };

            Thread listenerThread = new Thread(myObjListener.Listen);
            listenerThread.Start();
        }

        protected override void OnStop()
        {
            myObjListener.StopListen();
        }
    }
}
