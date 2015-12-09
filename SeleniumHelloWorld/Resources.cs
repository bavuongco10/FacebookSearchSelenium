namespace SeleniumHelloWorld
{
    internal class Resources
    {
        public Resources()
        {
            userName = "01646990322";
            pass = "vugon1994";
        }

        public Resources(string username, string pass)
        {
            userName = username;
            this.pass = pass;
        }

        public string userName { get; private set; }
        public string pass { get; private set; }
    }
}