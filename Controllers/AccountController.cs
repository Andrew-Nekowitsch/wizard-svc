using Microsoft.AspNetCore.Mvc;
using Data.Queries;
using Services;
using Models.Requests;

namespace Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

}