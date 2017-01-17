using System;
using System.Threading;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace brispark
{
	public class MessageEvent : EventArgs
	{
		public string	key;
		public JObject	value;
	}

	public class socketIO : IDisposable
	{
		private Timer        _timer;
		private WebSocket    _websocket;

		#region Public Events
		public event EventHandler onOpen;
		public event EventHandler<MessageEvent> onMessage;
		public event EventHandler<ErrorEventArgs> onError;
		public event EventHandler<CloseEventArgs> onClose;
		#endregion

		// Implement a call with the right signature for events going off
		private void _send_ping(object source)
		{
			_websocket.Send ("2");
		}
		private void _sio_session(string data)
		{
			var emt	= data [0];
			var json	= JObject.Parse (data.Substring (1));
			int pintv	= (int)json.GetValue ("pingInterval");
			_timer.Change (pintv, pintv);
		}
		private void _onOpen(object sender, EventArgs e)
		{
			onOpen.Emit (this, e);
			emit ("add user", "world");
		}
		private void _onClose(object sender, CloseEventArgs e)
		{
			_timer.Change(Timeout.Infinite, Timeout.Infinite);
			onClose.Emit (this, e);
		}
		private void _onMessage(object sender, MessageEventArgs e)
		{
			//Console.WriteLine (e.Data);
			var emt	= e.Data [0];
			switch(emt) {
			case '0':	_sio_session (e.Data); return;		// session info
			case '1': return;									// close
			case '2': return;									// ping
			case '3': return;									// pong
			case '4': break;									// message
			case '5': return;									// upgrade
			case '6': return;									// noop
			default:	return;
			};

			var smt	= e.Data [1];
			switch (smt) {
			case '0': return;									// connect
			case '1': return;									// disconnect
			case '2': break;									// event
			case '3': return;									// ack
			case '4': return;									// error
			case '5': return;									// binary event
			case '6': return;									// binary ack
			default:	return;
			};

			JArray m	= JArray.Parse(e.Data.Substring (2));
			//Console.WriteLine ("  "+ m [0] + "\n" + m[1].ToString());

			MessageEvent	me	= new MessageEvent();
			me.key	= m [0].ToString();
			me.value	= m [1].ToObject<JObject>();
			onMessage.Emit (this, me);
		}
		private void _onError(object sender, ErrorEventArgs e)
		{
			Close ();
			onError.Emit (this, e);
		}

		public void emit(string key, JObject val)
		{
			JArray	r1	= new JArray ();
			r1.Add (key);
			r1.Add (val);
			_websocket.Send("42"+ r1.ToString());
		}
		public void emit(string key, string val)
		{
			JArray	r1	= new JArray ();
			r1.Add (key);
			r1.Add (val);
			_websocket.Send("42"+ r1.ToString());
		}

		public socketIO()
		{
			_websocket	= new WebSocket ("ws://t1.brispark.com/socket.io/?EIO=2&transport=websocket");
			_websocket.OnOpen	+= _onOpen;
			_websocket.OnClose += _onClose;
			_websocket.OnMessage += _onMessage;
			_websocket.OnError += _onError;

			_timer = new Timer (_send_ping);
		}
		public int Connect()
		{
			_websocket.Connect ();
			return	0;
		}
		public int Close()
		{
			_timer.Dispose ();
			_websocket.Close ();
			return	0;
		}
		void IDisposable.Dispose ()
		{
			Close ();
		}
	}
}
