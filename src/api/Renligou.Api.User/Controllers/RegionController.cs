using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Renligou.Core.Application.Kernel;
using Renligou.Core.Application.Kernel.Queries;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Api.User.Controllers
{
    /// <summary>
    /// 行政区划
    /// </summary>
    [ApiController]
    [Route("/regions")]
    public class RegionController : Controller
    {
        private readonly RegionFacade _regionFacade;

        public RegionController(RegionFacade regionFacade)
        {
            _regionFacade = regionFacade;
        }

        /// <summary>
        /// 搜索行政区划列表
        /// </summary>
        /// <param name="parentId">父区划ID</param>
        /// <param name="regionName">区划名称关键词（模糊匹配，可搜索拼音）</param>
        /// <response code="200">获取成功</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<RegionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<RegionDto>>> Index(
            [FromQuery] long parentId,
            [FromQuery] string? regionName
        )
        {

            return await _regionFacade.GetRegionListAsync(parentId, regionName);
        }
    }
}
