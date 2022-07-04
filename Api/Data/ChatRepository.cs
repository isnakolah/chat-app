using System.Linq.Expressions;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Api.Models;
using Expression = System.Linq.Expressions.Expression;

namespace Api.Data;

internal sealed class ChatRepository : IChatRepository
{
    private readonly IDynamoDBContext _context;

    public ChatRepository(IDynamoDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Chat>> GetAllAsync()
    {
        return await _context.ScanAsync<Chat>(default).GetRemainingAsync();
    }

    public async Task<IEnumerable<Chat>> GetAllAsync(Expression<Func<Chat, bool>> expression)
    {
        var conditions = GetConditions(expression);

        return await _context.ScanAsync<Chat>(conditions).GetRemainingAsync();
    }

    public async Task AddAsync(Chat chat)
    {
        await _context.SaveAsync(chat);
    }

    private static IEnumerable<ScanCondition> GetConditions(Expression<Func<Chat, bool>> expression)
    {
        var conditions = new List<ScanCondition>();

        if (expression.Body is not BinaryExpression binaryExpression) 
            return conditions;

        foreach (var exp in new[] { binaryExpression.Left, binaryExpression.Right })
        {
            if (exp is not BinaryExpression memberExpression)
                continue;

            if (TryGetValue(memberExpression, out var fieldName, out var value, out var scanOperator))
                conditions.Add(new ScanCondition(fieldName, scanOperator, value));
        }

        return conditions;
    }
    
    private static bool TryGetValue(BinaryExpression binaryExpression, out string? fieldName, out string? value, out ScanOperator scanOperator)
    {
        (fieldName, value, scanOperator) = (null, null, ScanOperator.IsNull);

        if (binaryExpression.Right is not MemberExpression rightMemberExpression)
            return false;

        if (binaryExpression.Left is not MemberExpression leftMemberExpression)
            return false;

        if (rightMemberExpression.Expression is not ConstantExpression leftMemberConstantExpression)
            return false;

        fieldName = leftMemberExpression.Member.Name;

        value = leftMemberConstantExpression.Value!
            .GetType()
            .GetField(fieldName.ToLower())!
            .GetValue(leftMemberConstantExpression.Value) as string;

        scanOperator = binaryExpression.NodeType switch
        {
            ExpressionType.Equal => ScanOperator.Equal,
            ExpressionType.NotEqual => ScanOperator.NotEqual,
            _ => throw new Exception("No scan operator configured")
        };

        return true;
    }
}
