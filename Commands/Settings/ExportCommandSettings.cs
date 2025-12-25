using System.CommandLine;
using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public class ExportCommandSettings : BaseSettings<ExportCommandSettings>
    {
        public string? Type { get; set; }
        public string? Source { get; set; }
        public string? S3AccessKey { get; set; }
        public string? S3SecretKey { get; set; }
        public string? S3Endpoint { get; set; }
        public string? Bucket { get; set; }
        public string? Key { get; set; }
        public string? Directory { get; set; }
        public bool SaveCredentials { get; set; }

        private readonly Option<bool> _extraOption;
        private readonly Option<string?> _typeOption;
        private readonly Option<string?> _sourceOption;
        private readonly Option<string?> _s3AccessKeyOption;
        private readonly Option<string?> _s3SecretKeyOption;
        private readonly Option<string?> _s3EndpointOption;
        private readonly Option<string?> _bucketOption;
        private readonly Option<string?> _keyOption;
        private readonly Option<string?> _directoryOption;
        private readonly Option<bool> _saveCredentialsOption;

        public ExportCommandSettings()
        {
            _extraOption = new Option<bool>("--extra", "Displays extra information including how long the application took to run.");
            _typeOption = new Option<string?>(new[] { "-t", "--type" }, "Export type: remote (S3) or local (filesystem).");
            _sourceOption = new Option<string?>(new[] { "-s", "--source" }, "Bookmark source to export: chrome, brave, edge, opera, markdown, html, or all.");
            _s3AccessKeyOption = new Option<string?>("--s3-access-key", "S3 access key.");
            _s3SecretKeyOption = new Option<string?>("--s3-secret-key", "S3 secret key.");
            _s3EndpointOption = new Option<string?>("--s3-endpoint", "S3 endpoint URL.");
            _bucketOption = new Option<string?>("--bucket", "S3 bucket name.");
            _keyOption = new Option<string?>("--key", "S3 object key/path.");
            _directoryOption = new Option<string?>(new[] { "-d", "--directory" }, "Local directory path for export.");
            _saveCredentialsOption = new Option<bool>("--save-credentials", "Save S3 credentials to config.");
        }

        protected override ExportCommandSettings GetBoundValue(BindingContext bindingContext)
        {
            return new ExportCommandSettings
            {
                Extra = bindingContext.ParseResult.GetValueForOption(_extraOption),
                Type = bindingContext.ParseResult.GetValueForOption(_typeOption),
                Source = bindingContext.ParseResult.GetValueForOption(_sourceOption),
                S3AccessKey = bindingContext.ParseResult.GetValueForOption(_s3AccessKeyOption),
                S3SecretKey = bindingContext.ParseResult.GetValueForOption(_s3SecretKeyOption),
                S3Endpoint = bindingContext.ParseResult.GetValueForOption(_s3EndpointOption),
                Bucket = bindingContext.ParseResult.GetValueForOption(_bucketOption),
                Key = bindingContext.ParseResult.GetValueForOption(_keyOption),
                Directory = bindingContext.ParseResult.GetValueForOption(_directoryOption),
                SaveCredentials = bindingContext.ParseResult.GetValueForOption(_saveCredentialsOption)
            };
        }

        internal void AddOptionsToCommand(Command command)
        {
            command.AddOption(_extraOption);
            command.AddOption(_typeOption);
            command.AddOption(_sourceOption);
            command.AddOption(_s3AccessKeyOption);
            command.AddOption(_s3SecretKeyOption);
            command.AddOption(_s3EndpointOption);
            command.AddOption(_bucketOption);
            command.AddOption(_keyOption);
            command.AddOption(_directoryOption);
            command.AddOption(_saveCredentialsOption);
        }
    }
}
