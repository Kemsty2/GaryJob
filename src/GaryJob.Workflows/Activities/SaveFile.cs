using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;
using GaryJob.Core.Entities.FileAggregate;
using GaryJob.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GaryJob.Workflows.Activities
{
    [Action(Category = "File Management", Description = "Save data into a file")]
    public class SaveFile : Activity
    {
        #region Input Attributes

        [ActivityInput(
            Label = "FileName",
            Hint = "Name of the file",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string FileName { get; set; }

        [ActivityInput(
            Label = "SavePath",
            Hint = "Path to save the file",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string SavePath { get; set; }

        #endregion Input Attributes

        #region Output Attributes

        [ActivityOutput(Hint = "File Created", Name = "File")]
        public FileModel File { get; set; }

        #endregion Output Attributes

        private readonly IFileStorage _fileStorage;
        private readonly IMimeMappingService _mapping;

        public SaveFile(IFileStorage fileStorage, IMimeMappingService mapping)
        {
            _fileStorage = fileStorage;
            _mapping = mapping;
        }

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            //TODO: Save the document with the file name provide and on the specific savepath

            var input = context.GetInput<dynamic>();

            if (input == null)
                return Outcome("Please provide data");

            if (!Directory.Exists(SavePath))
            {
                return Outcome("Please provide data");
            }

            var body = input.Body;

            var bytes = await _fileStorage.SaveFileAsync(FileName, SavePath, new List<dynamic> { body }, context.CancellationToken);

            File = new FileModel
            {
                FileName = FileName,
                Content = bytes,
                MimeType = _mapping.Map(FileName)
            };

            return Done();
        }
    }
}