using System.Text;

namespace SqlD.Options;

public class CommandParser
{
    public static void Info<T>() where T : new()
    {
        var builder = new StringBuilder();

        var type = typeof(T);

        var commandAttribute = type.GetCustomAttributes(typeof(CommandAttribute), false).Cast<CommandAttribute>().FirstOrDefault();
        if (commandAttribute != null)
        {
            builder.AppendLine();
            builder.AppendLine($"{commandAttribute.Command}");
            builder.AppendLine(new string('-', commandAttribute.Command.Length));
        }

        builder.AppendLine();

        foreach (var propertyInfo in type.GetProperties())
        {
            var argumentAttribute = propertyInfo.GetCustomAttributes(typeof(ArgumentAttribute), false).Cast<ArgumentAttribute>().FirstOrDefault();
            if (argumentAttribute != null)
            {
                builder.AppendLine(string.Format($"   {argumentAttribute.LongName} {argumentAttribute.ShortName}"));
                builder.AppendLine(string.Format($"      {argumentAttribute.Help}"));
                builder.AppendLine();
            }
        }

        Console.WriteLine(builder.ToString());
    }
}