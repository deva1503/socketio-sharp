using System;
using WebSocketSharp;
using System.Timers;

namespace brispark
{
	public class socketIO : IDisposable
	{
		private Timer        _timer;
		private WebSocket    _websocket;

		#region Public Events
		public event EventHandler onOpen;
		public event EventHandler<MessageEventArgs> onMessage;
		public event EventHandler<ErrorEventArgs> onError;
		public event EventHandler<CloseEventArgs> onClose;
		#endregion

		// Implement a call with the right signature for events going off
		private void _sendPing(object source, ElapsedEventArgs e)
		{
			_websocket.Send("2");
		}
		private void _onOpen(object sender, EventArgs e)
		{
			_timer.Start ();
		}
		private void _onClose(object sender, CloseEventArgs e)
		{
			_timer.Stop ();
		}
		private void _onMessage(object sender, MessageEventArgs e)
		{
		}
		private void _onError(object sender, ErrorEventArgs e)
		{
			_timer.Stop ();
		}

		public socketIO()
		{
			_websocket	= new WebSocket ("ws://bingo.fann.co.kr/socket.io/?EIO=2&transport=websocket");
			_websocket.OnOpen	+= _onOpen;
			_websocket.OnClose += _onClose;
			_websocket.OnMessage += _onMessage;
			_websocket.OnError += _onError;

			_timer = new Timer (_sendPing);
			_timer.Interval	= 30000;
			_timer.Enabled	= false;
		}
		public int connect()
		{
			_websocket.Connect ();
			return	0;
		}
		public int close()
		{
			_timer.Close ();
			_websocket.Close ();
			return	0;
		}
	}
}
