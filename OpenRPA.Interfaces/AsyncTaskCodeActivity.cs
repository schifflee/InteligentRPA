﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRPA.Interfaces
{
    public abstract class AsyncTaskCodeActivity : AsyncCodeActivity
    {
        protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var task = ExecuteAsync(context);
            var tcs = new TaskCompletionSource<object>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);
                callback?.Invoke(tcs.Task);
            });
            return tcs.Task;
        }

        protected sealed override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            var task = (Task<object>)result;
            try
            {
                AfterExecute(context, task.Result);
                return;
            }
            catch (AggregateException ex)
            {
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
        protected abstract Task<object> ExecuteAsync(AsyncCodeActivityContext context);
        protected abstract void AfterExecute(AsyncCodeActivityContext context, object result);
    }

    public abstract class AsyncTaskCodeActivity<T> : AsyncCodeActivity<T>
    {
        protected sealed override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var task = ExecuteAsync(context);
            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);

                if (callback != null)
                    callback(tcs.Task);
            });

            return tcs.Task;
        }

        protected sealed override T EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            var task = (Task<T>)result;
            try
            {
                //return task.Result;
                return PostExecute(context, task.Result);
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }
        //https://stackoverflow.com/questions/26054585/asynctaskcodeactivity-and-lost-context-after-await
        protected virtual T PostExecute(AsyncCodeActivityContext context, T result)
        {
            return result;
        }
        protected abstract Task<T> ExecuteAsync(AsyncCodeActivityContext context);
    }


    //public abstract class AsyncTaskNativeActivity : AsyncNativeActivity
    //{
    //    protected sealed override IAsyncResult BeginExecute(NativeActivityContext context, AsyncCallback callback, object state)
    //    {
    //        var task = ExecuteAsync(context);
    //        var tcs = new TaskCompletionSource<bool>(state);
    //        task.ContinueWith(t =>
    //        {
    //            if (t.IsFaulted)
    //                tcs.TrySetException(t.Exception.InnerExceptions);
    //            else if (t.IsCanceled)
    //                tcs.TrySetCanceled();
    //            //else
    //            //    tcs.TrySetResult(t.Result);

    //            if (callback != null)
    //                callback(tcs.Task);
    //        });

    //        return tcs.Task;
    //    }

    //    protected sealed override void EndExecute(NativeActivityContext context, IAsyncResult result)
    //    {
    //        var task = (Task)result;
    //        try
    //        {
    //            task.Wait();
    //        }
    //        catch (AggregateException ex)
    //        {
    //            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
    //            throw;
    //        }
    //    }

    //    protected abstract Task ExecuteAsync(NativeActivityContext context);
    //}

}
