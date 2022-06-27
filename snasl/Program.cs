using System;
using snasl.Lang;
using snasl.Lang.Emitters;
using snasl.Lang.Parser;

namespace snasl
{
    class Program
    {
        static void Main (string[] args)
        {
            var t = new NaslTokenizer ();
 
            var path = @"/mnt/nvt-feed/2019/amcrest/gb_amcrest_ip_camera_default_credentials.nasl";

            try
            {
                var text = System.IO.File.ReadAllText (path);
                var p = new NaslParser (t.Tokenize (text));
                p.Parse ();
            }
            catch (Exception ex)
            {
                Console.WriteLine ("ERR:" + ex.Message);
                Console.WriteLine ();

                throw;
            }
        }
    }
}
