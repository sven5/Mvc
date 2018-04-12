// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPagesWebSite.Pages
{
    [AsyncTestFiltersAttributeAttribute]
    [SyncTestFiltersAttributeAttribute]
    public class ShortCircuitPageModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }

        private static bool ShouldShortCircuit(HttpContext httpContext, string currentTargetName)
        {
            return httpContext.Request.Query.TryGetValue("target", out var expectedTargetName)
                && string.Equals(expectedTargetName, currentTargetName, StringComparison.OrdinalIgnoreCase);
        }

        private class AsyncTestFiltersAttributeAttributeAttribute :
            Attribute,
            IAsyncAuthorizationFilter,
            IAsyncResourceFilter,
            IAsyncPageFilter,
            IAsyncResultFilter
        {
            public Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnAuthorizationAsync)))
                {
                    context.Result = new PageResult();
                }
                return Task.CompletedTask;
            }

            public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnResourceExecutionAsync)))
                {
                    context.Result = new PageResult();
                    return Task.CompletedTask;
                }
                return next();
            }

            public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
            {
                return Task.CompletedTask;
            }

            public Task OnPageHandlerExecutionAsync(
                PageHandlerExecutingContext context,
                PageHandlerExecutionDelegate next)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnPageHandlerExecutionAsync)))
                {
                    context.Result = new PageResult();
                    return Task.CompletedTask;
                }
                return next();
            }

            public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnResultExecutionAsync)))
                {
                    context.Result = new PageResult();
                }
                return next();
            }
        }

        private class SyncTestFiltersAttributeAttributeAttribute :
            Attribute,
            IAuthorizationFilter,
            IResourceFilter,
            IPageFilter,
            IResultFilter
        {
            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnAuthorization)))
                {
                    context.Result = new PageResult();
                }
            }

            public void OnResourceExecuting(ResourceExecutingContext context)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnResourceExecuting)))
                {
                    context.Result = new PageResult();
                }
            }

            public void OnResourceExecuted(ResourceExecutedContext context)
            {
            }

            public void OnPageHandlerSelected(PageHandlerSelectedContext context)
            {
            }

            public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnPageHandlerExecuting)))
                {
                    context.Result = new PageResult();
                }
            }

            public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
            {
            }

            public void OnResultExecuting(ResultExecutingContext context)
            {
                if (ShouldShortCircuit(context.HttpContext, nameof(OnResultExecuting)))
                {
                    context.Result = new PageResult();
                }
            }

            public void OnResultExecuted(ResultExecutedContext context)
            {
            }
        }
    }
}
