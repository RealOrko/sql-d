namespace SqlD.Extensions.Threading;

public static class TaskExtensions
{
    public static IEnumerable<TOutput> ForEachTask<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, TOutput> taskAction)
    {
        return inputs.ForEachTaskWithMany(x =>
        {
            var result = taskAction(x);
            return new[] { result };
        });
    }

    public static async Task<IEnumerable<TOutput>> ForEachTaskAsync<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, Task<TOutput>> taskActionAsync)
    {
        return await inputs.ForEachTaskWithManyAsync<TInput, TOutput>(async x =>
        {
            var result = await taskActionAsync(x);
            return new[] { result };
        });
    }

    public static IEnumerable<TOutput> ForEachTaskWithMany<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, IEnumerable<TOutput>> taskAction)
    {
        var tasks = new List<Task>();
        var synchronise = new object();
        var outputs = new List<TOutput>();

        foreach (var input in inputs)
            tasks.Add(Task.Factory.StartNew(() =>
            {
                var output = taskAction(input);
                lock (synchronise)
                {
                    outputs.AddRange(output);
                }
            }));

        // TODO: Implement proper error handling of tasks
        Task.WaitAll(tasks.ToArray());

        return outputs;
    }

    public static async Task<IEnumerable<TOutput>> ForEachTaskWithManyAsync<TInput, TOutput>(this IEnumerable<TInput> inputs, Func<TInput, Task<IEnumerable<TOutput>>> taskActionAsync)
    {
        var tasks = new List<Task>();
        var synchronise = new object();
        var outputs = new List<TOutput>();

        foreach (var input in inputs)
            tasks.Add(await Task.Factory.StartNew(async () =>
            {
                var output = await taskActionAsync(input);
                lock (synchronise)
                {
                    outputs.AddRange(output);
                }
            }));

        // TODO: Implement proper error handling of tasks
        Task.WaitAll(tasks.ToArray());

        return outputs;
    }
}