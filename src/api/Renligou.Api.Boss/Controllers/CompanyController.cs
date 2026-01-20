using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers
{
    /// <summary>
    /// 公司
    /// </summary>
    [ApiController]
    [Route("/companies")]
    public class CompanyController(
        ICommandBus _commandBus,
        IQueryBus _queryBus,
        IUnitOfWork _uow,
        ILogger<CompanyController> _logger
    ) : Controller
    {
        /// <summary>
        /// 创建 公司/分公司/ 子公司
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken = default)
        {
            DateOnly? effectiveDate = string.IsNullOrWhiteSpace(request.EffectiveDate)
                ? null
                : DateOnly.Parse(request.EffectiveDate!);
            DateOnly? expiredDate = string.IsNullOrWhiteSpace(request.ExpiredDate)
                ? null
                : DateOnly.Parse(request.ExpiredDate!);

            var command = new CreateCompanyCommand
            {
                CompanyType = request.CompanyType,
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                CompanyShortName = request.CompanyShortName,
                LegalPersonName = request.LegalPersonName,
                CreditCode = request.CreditCode,
                RegisteredAddress = request.RegisteredAddress,
                Remark = request.Remark,
                ProvinceId = request.ProvinceId,
                CityId = request.CityId,
                DistrictId = request.DistrictId,
                CompletedAddress = request.CompletedAddress,
                Enabled = request.Enabled,
                EffectiveDate = effectiveDate,
                ExpiredDate = expiredDate
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<CreateCompanyCommand, Result>(command, cancellationToken);
            }, true);
            if (!res.Success)
                return BadRequest(res.Error);

            return Ok();
        }

        /// <summary>
        /// 修改 公司/分公司/子公司 基础信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/basic")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyBasic(
            [FromRoute] long id,
            [FromBody] ModifyCompanyBasicRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var command = new ModifyCompanyBasicCommand
            {
                CompanyId = id,
                CompanyType = request.CompanyType,
                CompanyCode = request.CompanyCode,
                CompanyName = request.CompanyName,
                CompanyShortName = request.CompanyShortName,
                LegalPersonName = request.LegalPersonName,
                CreditCode = request.CreditCode,
                RegisteredAddress = request.RegisteredAddress,
                Remark = request.Remark
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<ModifyCompanyBasicCommand, Result>(command, cancellationToken);
            }, true);
            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok();
        }

        /// <summary>
        /// 修改 公司/分公司/子公司 地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/address")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyAddress(
            [FromRoute] long id,
            [FromBody] ModifyCompanyAddressRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var command = new ModifyCompanyAddressCommand
            {
                CompanyId = id,
                ProvinceId = request.ProvinceId,
                CityId = request.CityId,
                DistrictId = request.DistrictId,
                CompletedAddress = request.CompletedAddress
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<ModifyCompanyAddressCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok();
        }

        /// <summary>
        /// 修改 公司/分公司/子公司 状态
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("{id:long}/state")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyState(
            [FromRoute] long id,
            [FromBody] ModifyCompanyStateRequest request,
            CancellationToken cancellationToken = default
        )
        {
            DateOnly? effectiveDate = string.IsNullOrWhiteSpace(request.EffectiveDate)
                ? null
                : DateOnly.Parse(request.EffectiveDate!);
            DateOnly? expiredDate = string.IsNullOrWhiteSpace(request.ExpiredDate)
                ? null
                : DateOnly.Parse(request.ExpiredDate!);

            var command = new ModifyCompanyStateCommand
            {
                CompanyId = id,
                Enabled = request.Enabled,
                EffectiveDate = effectiveDate,
                ExpiredDate = expiredDate
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<ModifyCompanyStateCommand, Result>(command, cancellationToken);
            });
            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok();
        }

        /// <summary>
        /// 获取 公司/分公司/子公司 详情
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(CompanyDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDetail(
            [FromRoute] long id,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetCompanyDetailQuery(id);
            var res = await _queryBus.QueryAsync<GetCompanyDetailQuery, Result<CompanyDetailDto?>>(query, cancellationToken);
            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }

        /// <summary>
        /// 获取 公司/分公司/子公司 列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CompanyListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// 获取 公司/分公司/子公司 分页
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("pagination")]
        [ProducesResponseType(typeof(Pagination<CompanyDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPage(
            [FromQuery] GetCompanyPageRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetCompanyPageQuery(
                request.CompanyType,
                request.CompanyName,
                request.ProvinceId,
                request.Status,
                request.Actived,
                request.Page,
                request.PageSize
            );

            var res = await _queryBus.QueryAsync<GetCompanyPageQuery, Result<Pagination<CompanyDetailDto>>>(query, cancellationToken);

            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }
    }
}
