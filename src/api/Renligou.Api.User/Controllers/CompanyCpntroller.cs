using Microsoft.AspNetCore.Mvc;
using Renligou.Api.User.Requests;
using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.User.Controllers
{
    /// <summary>
    /// 公司
    /// </summary>
    [ApiController]
    [Route("/companies")]
    public class CompanyCpntroller(
        IQueryBus _queryBus,
        IUnitOfWork _uow
    ) : ControllerBase
    {
        /// <summary>
        /// 获取 公司/分公司/子公司 列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetCompanyListRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetCompanyListQuery(
                null,
                request.CompanyName,
                null,
                null,
                null
            );

            var res = await _queryBus.QueryAsync<GetCompanyListQuery, Result<List<CompanyListDto>>>(query, cancellationToken);
            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }
    }
}
