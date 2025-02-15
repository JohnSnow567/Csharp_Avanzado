using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;


namespace CapaAplicacion.Services.TaskServices
{
    public class ReactiveTaskQueue : IDisposable
    {
        private readonly ISubject<Func<Task>> _taskSubject;
        private readonly IDisposable _subscription;

        public ReactiveTaskQueue()
        {
            _taskSubject = new Subject<Func<Task>>();
            _subscription = _taskSubject
                .Select(task => Observable.FromAsync(task))
                .Concat()
                .Subscribe();
        }

        /// <summary>
        /// Encola una tarea que no retorna valor.
        /// </summary>
        public Task Enqueue(Func<Task> task)
        {
            var tcs = new TaskCompletionSource<object>();
            _taskSubject.OnNext(async () =>
            {
                try
                {
                    await task();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Encola una tarea que retorna un valor de tipo T.
        /// </summary>
        public Task<T> Enqueue<T>(Func<Task<T>> task)
        {
            var tcs = new TaskCompletionSource<T>();
            _taskSubject.OnNext(async () =>
            {
                try
                {
                    T result = await task();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }

        public void Dispose()
        {
            _subscription.Dispose();
            _taskSubject.OnCompleted();
        }
    }
}



