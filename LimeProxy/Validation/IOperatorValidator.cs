namespace LimeProxy.Validation
{
    public interface IOperatorValidator
    {
        bool IsValid(string op);
    }

    public class OperatorValidator : IOperatorValidator
    {
        public bool IsValid(string op)
        {
            switch (op.ToUpper())
            {
                case "=":
                case ">":
                case "<":
                case ">=":
                case "<=":
                case "!=":
                case "LIKE":
                case "IN":
                case "ANY":
                case "LIKE%":
                case "%LIKE":
                case "NOT LIKE":
                case "NOT LIKE%":
                case "NOT %LIKE":
                case "NOT IN":
                case "IS":
                case "IS NOT":
                    return true;

                default:
                    return false;
            }
        }
    }
}