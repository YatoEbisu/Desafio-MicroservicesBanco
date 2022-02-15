using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Notie.Contracts;

namespace ModalMais.Conta.Api.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;
        protected readonly AbstractNotifier _notifier;

        public BaseController(IMapper mapper, AbstractNotifier notifier)
        {
            _mapper = mapper;
            _notifier = notifier;
        }
    }
}