using Elsa.Scripting.JavaScript.Services;
using System;
using System.Collections.Generic;

namespace GaryJob.Workflows.Scripting.JavaScript
{
    public class CustomTypeDefinitionProvider : TypeDefinitionProvider
    {
        public override IEnumerable<Type> CollectTypes(TypeDefinitionContext context)
        {
            yield return null;
        }
    }
}