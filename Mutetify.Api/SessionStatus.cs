namespace Mutetify.Api
{
    public class SessionStatus
    {
        #region Fields
        private bool _running;
        private bool _playEnabled;
        private bool _nextEnabled;
        private bool _previousEnabled;
        #endregion

        #region Properties
        public bool Running
        {
            get { return _running; }
            set { _running = value; }
        }

        public bool NextEnabled
        {
            get { return _nextEnabled; }
            set { _nextEnabled = value; }
        }

        public bool PreviousEnabled
        {
            get { return _previousEnabled; }
            set { _previousEnabled = value; }
        }

        public bool PlayEnabled
        {
            get { return _playEnabled; }
            set { _playEnabled = value; }
        }
        #endregion

    }
}
