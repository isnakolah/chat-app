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

        if (binaryExpression.Left is not BinaryExpression leftMemberExpression)
            return conditions;

        var (fieldName, value) = GetValue(leftMemberExpression);

        var scanOperator = leftMemberExpression.NodeType switch
        {
            ExpressionType.Equal => ScanOperator.Equal,
            ExpressionType.NotEqual => ScanOperator.NotEqual
        };

        if (fieldName is not null && value is not null)
            conditions.Add(new ScanCondition(fieldName, scanOperator, value));

        return conditions;
    }
    
    private static (string? FieldName, string? Value) GetValue(BinaryExpression binaryExpression)
    {
        if (binaryExpression.Right is not MemberExpression rightMemberExpression)
            return (null, null);

        if (binaryExpression.Left is not MemberExpression leftMemberExpression)
            return (null, null);

        if (rightMemberExpression.Expression is not ConstantExpression leftMemberConstantExpression)
            return (null, null);

        var fieldName = leftMemberExpression.Member.Name;

        var value = leftMemberConstantExpression.Value!.GetType().GetField(fieldName.ToLower())!.GetValue(leftMemberConstantExpression.Value);

        return (fieldName, (string)value!);
    }
}
