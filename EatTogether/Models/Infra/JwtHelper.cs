using EatTogether.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EatTogether.Models.Infra
{
	public class JwtHelper
	{
		private readonly IConfiguration _config;

		public JwtHelper(IConfiguration config)
		{
			_config = config;
		}

		/// <summary>
		/// 產生 JWT，Payload 包含 UserId 與 RoleIds 清單
		/// </summary>
		/// <param name="userId">使用者 Id</param>
		/// <param name="roleIds">角色 Id 清單</param>
		/// <returns>回傳 JWT 字串</returns>
		public string GenerateToken(JwtPayloadDto payloadDto)
		{
			// 讀取 JWT 設定
			var jwtSettings = _config.GetSection("Jwt");
			var secretKey = jwtSettings["SecretKey"]!;
			var issuer = jwtSettings["Issuer"]!;
			var audience = jwtSettings["Audience"]!;

			// 員工頭像隨機背景色
			var colors = new[]
			{
				"#C9A96E", "#8B5E3C", "#A0522D", "#6B4226",
				"#B07D4E", "#7A4F2D", "#9C6B3C", "#5C3317"
			};

			// 依名字雜湊決定顏色，確保同一使用者每次顏色相同
			int hash = 0;
			foreach (var c in payloadDto.Name)
				hash = (hash * 31 + c) & 0x7fffffff;

			var avatarColor = colors[hash % colors.Length];

			// 建立 Claims (Payload 鍵值對)
			var claims = new List<Claim>
			{
				new Claim("userId", payloadDto.UserId.ToString()),
				new Claim("name", payloadDto.Name),
				new Claim("avatarColor", avatarColor),
				// 角色名稱全部用 | 串接後存成一筆
				new Claim("roleNames", string.Join(" | ", payloadDto.RoleNames))
			};

			foreach (var roleId in payloadDto.RoleIds)	
			{
				claims.Add(new Claim("roleId", roleId.ToString()));
			}

			//把每個 FunctionName 寫成獨立 Claim
			foreach (var fn in payloadDto.FunctionNames)
			{
				claims.Add(new Claim("Permission", fn));
			}

			// JWT 所需的安全金鑰物件
			var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

			// 建立 JWT 的簽章憑證
			var credentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256);

			// 建立 JWT 物件
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(8),
				signingCredentials: credentials
			);

			// 將 JWT 物件轉換成標準的 JWT 字串格式並回傳
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
