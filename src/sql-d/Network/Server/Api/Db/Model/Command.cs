using SqlD.Extensions;

namespace SqlD.Network.Server.Api.Db.Model
{
	public class Command
	{
		public List<string> Commands { get; set; } = new List<string>();

		public Command AddInsert(object instance, bool withIdentity = false)
		{
			Commands.Add(instance.GetInsert(withIdentity));
			return this;
		}

		public Command AddManyInserts(IEnumerable<object> instances, bool withIdentity = false)
		{
			Commands.AddRange(instances.Select(x => x.GetInsert(withIdentity)));
			return this;
		}

		public Command AddUpdate(object instance)
		{
			Commands.Add(instance.GetUpdate());
			return this;
		}

		public Command AddManyUpdates(IEnumerable<object> instances)
		{
			Commands.AddRange(instances.Select(x => x.GetUpdate()));
			return this;
		}

		public Command AddDelete(object instance)
		{
			Commands.Add(instance.GetDelete());
			return this;
		}

		public Command AddManyDeletes(IEnumerable<object> instances)
		{
			Commands.AddRange(instances.Select(x => x.GetDelete()));
			return this;
		}

		public void EnsureTableExists(object instance)
		{
			var createTable = instance.GetType().GetCreateTable();
			Commands.Add(createTable);
		}
	}
}