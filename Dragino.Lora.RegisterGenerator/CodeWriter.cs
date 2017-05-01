using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dragino.Lora.RegisterGenerator
{
    public class CodeWriter
    {
        private const string INDENT = "    ";
        private readonly IReferenceResolver _referenceResolver;
        private readonly bool _suppressComments;
        private readonly StringBuilder _code = new StringBuilder();

        public CodeWriter(IReferenceResolver referenceResolver, bool suppressComments = false)
        {
            _referenceResolver = referenceResolver;
            _suppressComments = suppressComments;
        }

        public int Indent { get; set; }

        public int CurrentPosition => _code.Length;

        public void AppendLine()
        {
            _code.AppendLine();
        }

        public void AppendLine(string code)
        {
            _code.AppendLine(string.Concat(Enumerable.Repeat(INDENT, Indent)) + code);
        }

        public void AppendCommentLine(string comment)
        {
            if (_suppressComments)
            {
                return;
            }

            _code.AppendLine(string.Concat(Enumerable.Repeat(INDENT, Indent)) + "// " + comment);
        }

        public void AppendDocumentationCommentLine(string comment)
        {
            if (_suppressComments)
            {
                return;
            }

            while (true)
            {
                Match referenceMatch = Regex.Match(comment, "^.*{(.*)}.*$");
                if (!referenceMatch.Success)
                {
                    break;
                }

                Group referenceGroup = referenceMatch.Groups[1];

                string reference = referenceGroup.Value;
                var commentPrefix = comment.Substring(0, referenceGroup.Index - 1);
                var commentSuffix = comment.Substring(referenceGroup.Index + referenceGroup.Length + 1);

                comment = $"{commentPrefix}<see cref=\"{_referenceResolver.ResolveReference(reference)}\"/>{commentSuffix}";
            }

            _code.AppendLine(string.Concat(Enumerable.Repeat(INDENT, Indent)) + "/// " + comment);
        }

        public string GetAllCode()
        {
            return _code.ToString();
        }
    }
}