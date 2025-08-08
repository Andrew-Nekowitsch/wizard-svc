using Microsoft.AspNetCore.Mvc;
using Data.Queries;
using Services;
using Models.Requests;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

}