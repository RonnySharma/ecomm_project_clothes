// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Mvc;

namespace ecomm_project_clothes.Areas.Identity.Pages.Account
{
    public interface ILoginModel
    {
        Task<IActionResult> OnGetCallbackAsync();
    }
}