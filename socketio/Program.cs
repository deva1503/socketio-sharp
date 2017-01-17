using System;
using brispark;
using System.Diagnostics;

namespace socketio
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var sio	= new socketIO ();
			sio.onOpen	+= (sender, e) =>
			{
				Console.WriteLine("onOpen "+ e.ToString());
			};
			sio.onClose += (sender, e) =>
			{
				Console.WriteLine("onClose "+ e.ToString());
			};
			sio.onError += (sender, e) =>
			{
				Console.WriteLine("onError "+ e.ToString());
			};
			sio.onMessage += (sender, e) =>
			{
				Console.WriteLine("onMessage "+ e.key +"\n"+ e.value.ToString());
			};

			sio.Connect ();

			Console.ReadLine ();
		}
	}
}
