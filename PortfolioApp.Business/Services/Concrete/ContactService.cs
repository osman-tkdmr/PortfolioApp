using AutoMapper;
using PortfolioApp.Business.Services.Interfaces;
using PortfolioApp.Core.Interfaces;
using PortfolioApp.Core.Results;
using PortfolioApp.DataAccess.UnitOfWork;
using PortfolioApp.DTO.DTOs.Site;
using PortfolioApp.Entity.Concrete;

namespace PortfolioApp.Business.Services.Concrete;

public class ContactService : IContactService
{
    private readonly UnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public ContactService(UnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<IDataResult<ContactInfoDto?>> GetContactInfoAsync()
    {
        var info = await _uow.GetRepository<ContactInfo>().FirstOrDefaultAsync(c => c.IsActive);
        return DataResult<ContactInfoDto?>.Ok(_mapper.Map<ContactInfoDto?>(info));
    }

    public async Task<IResult> UpdateContactInfoAsync(ContactInfoUpdateDto dto)
    {
        var info = await _uow.GetRepository<ContactInfo>().FirstOrDefaultAsync(c => c.Id == dto.Id && c.UserId == _currentUser.UserId);
        if (info is null) return Result.Fail("İletişim bilgisi bulunamadı.");
        _mapper.Map(dto, info);
        _uow.GetRepository<ContactInfo>().Update(info);
        await _uow.SaveChangesAsync();
        return Result.Ok("İletişim bilgileri güncellendi.");
    }

    public async Task<IDataResult<IList<ContactMessageDto>>> GetMessagesAsync()
    {
        var messages = await _uow.GetRepository<ContactMessage>()
            .FindAsync(m => !m.IsSpam && m.UserId == _currentUser.UserId);
        var ordered = messages.OrderByDescending(m => m.CreatedAt).ToList();
        return DataResult<IList<ContactMessageDto>>.Ok(_mapper.Map<IList<ContactMessageDto>>(ordered));
    }

    public async Task<IDataResult<ContactMessageDto>> GetMessageByIdAsync(int id)
    {
        var msg = await _uow.GetRepository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        return msg is null
            ? DataResult<ContactMessageDto>.Fail("Mesaj bulunamadı.")
            : DataResult<ContactMessageDto>.Ok(_mapper.Map<ContactMessageDto>(msg));
    }

    public async Task<IResult> SendMessageAsync(ContactMessageCreateDto dto)
    {
        var message = _mapper.Map<ContactMessage>(dto);
        await _uow.GetRepository<ContactMessage>().AddAsync(message);
        await _uow.SaveChangesAsync();
        return Result.Ok("Mesajınız gönderildi. En kısa sürede dönüş yapacağım.");
    }

    public async Task<IResult> MarkAsReadAsync(int id)
    {
        var msg = await _uow.GetRepository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        if (msg is null) return Result.Fail("Mesaj bulunamadı.");
        msg.IsRead = true;
        _uow.GetRepository<ContactMessage>().Update(msg);
        await _uow.SaveChangesAsync();
        return Result.Ok("Mesaj okundu olarak işaretlendi.");
    }

    public async Task<IResult> ReplyAsync(int id, string replyText)
    {
        var msg = await _uow.GetRepository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        if (msg is null) return Result.Fail("Mesaj bulunamadı.");
        msg.ReplyText = replyText;
        msg.IsReplied = true;
        msg.RepliedAt = DateTime.UtcNow;
        msg.IsRead = true;
        _uow.GetRepository<ContactMessage>().Update(msg);
        await _uow.SaveChangesAsync();
        return Result.Ok("Yanıt kaydedildi.");
    }

    public async Task<IResult> DeleteMessageAsync(int id)
    {
        var msg = await _uow.GetRepository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        if (msg is null) return Result.Fail("Mesaj bulunamadı.");
        await _uow.GetRepository<ContactMessage>().SoftDeleteAsync(id);
        await _uow.SaveChangesAsync();
        return Result.Ok("Mesaj silindi.");
    }

    public async Task<IResult> MarkAsSpamAsync(int id)
    {
        var msg = await _uow.GetRepository<ContactMessage>().FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);
        if (msg is null) return Result.Fail("Mesaj bulunamadı.");
        msg.IsSpam = true;
        _uow.GetRepository<ContactMessage>().Update(msg);
        await _uow.SaveChangesAsync();
        return Result.Ok("Mesaj spam olarak işaretlendi.");
    }
}
