﻿using System.Collections.Generic;
using SqlD.Network.Server;

namespace SqlD.Configuration.Model
{
	public class SqlDServiceModel : SqlDEndPointModel
	{
		private static readonly object Synchronised = new object();

		public string Name { get; set; }
		public string Database { get; set; }
		public List<string> Tags { get; set; } = new List<string>();
	    public SqlDPragmaModel Pragma { get; set; } = SqlDPragmaModel.Default;
		public List<SqlDForwardingModel> ForwardingTo { get; set; } = new List<SqlDForwardingModel>();
		public ConnectionListener Listener { get; private set; }

		internal void AddListener(ConnectionListener listener)
		{
			lock (Synchronised)
			{
				Listener = listener;
			}
		}

		public void DisposeListener()
		{
			lock (Synchronised)
			{
				Listener?.Dispose();
				Listener = null;
			}
		}

		public override string ToString()
		{
			return $"{nameof(Name)}: {Name}, {nameof(Database)}: {Database}";
		}
	}
}