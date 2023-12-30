namespace Chiats.SQL
{
    public class CommandTextModel : ICommandBuilder
    {
        public readonly NamedCollection<CommandParameter> Parameters = new NamedCollection<CommandParameter>();

        public CommandTextModel(string CommandText)
        {
            this.CommandText = CommandText;
        }

        public CommandTextModel(string CommandText, object[] args)
        {
            this.CommandText = string.Format(CommandText, args);
            //this.Parameters.Add( new CommandParameter( )
        }

        public string CommandText { get; set; }

        public virtual System.Data.CommandType CommandType
        {
            get { return System.Data.CommandType.Text; }
        }
    }
}