using System.Net.Http.Json;
using AcquaDiCane.Models.DTOs;

public class VeterinariaApiClient
{
    private readonly HttpClient _http;

    public VeterinariaApiClient(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("VeterinariaApi");
    }

    public async Task<List<DateTime>> ObtenerDisponibilidad(DateTime fecha)
    {
        var res = await _http.GetAsync($"api/turnos/disponibilidad?fecha={fecha:yyyy-MM-dd}");
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<List<DateTime>>();
    }

    public async Task<bool> CrearTurno(TurnoCreateDto dto)
    {
        var res = await _http.PostAsJsonAsync("api/turnos", dto);
        return res.IsSuccessStatusCode;
    }

    public async Task<List<TurnoResponseDto>> ObtenerTurnosPorDni(string dni)
    {
        var res = await _http.GetAsync($"api/turnos?dni={dni}");
        if (!res.IsSuccessStatusCode) return new List<TurnoResponseDto>();
        return await res.Content.ReadFromJsonAsync<List<TurnoResponseDto>>();
    }

    public async Task<bool> CancelarTurno(int id)
    {
        var res = await _http.DeleteAsync($"api/turnos/{id}");
        return res.IsSuccessStatusCode;
    }
}
