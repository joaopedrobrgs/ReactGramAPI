using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReactGramAPI.Data;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;
using ReactGramAPI.Services;
using System.IO;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace ReactGramAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PhotoController : ControllerBase
{

    private ReactgramDbContext _dbContext;
    private UserService _userService;
    private IMapper _mapper;

    public PhotoController(UserService userService, IMapper mapper, ReactgramDbContext dbContext)
    {
        _userService = userService;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    [HttpPost]
    [Authorize]
    [Route("upload")]
    public IActionResult UploadPhoto([FromForm] CreatePhotoDto photoDto)
    {
        try
        {
            ////Pegando usuário que está logado através do TOKEN:
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            ////Salvando foto no banco de dados:
            //Convertendo de CreatePhotoDto para Photo:
            Photo photo = _mapper.Map<Photo>(photoDto);
            //Adicionando id do usuário à foto:
            photo.UserId = user.Id;
            //Adicionando caminho e nome da foto:
            string path = System.IO.Path.Combine("photos", user.Id);
            string fileName = $"{DateTime.Now.Ticks}.jpg";
            photo.FilePath = System.IO.Path.Combine(path, fileName);

            //Salvando foto localmente no servidor:
            //Determinando diretório da foto:
            string absolutePath = System.IO.Path.Combine(Utils.Utils.GetPublicDir(path), fileName);
            //Salvando foto:
            using (Stream stream = new FileStream(absolutePath, FileMode.Create))
            {
                photoDto.File.CopyTo(stream);
            }

            //Adicionando foto ao contexto do banco de dados:
            _dbContext.Photo.Add(photo);
            //Salvando alteração no contexto do banco de dados:
            _dbContext.SaveChanges();

            ////Retornando se tudo tiver dado certo:
            return Ok("Foto Criada");
            //return CreatedAtAction(nameof(GetPhotoById), new { id = photo.Id }, photo);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    [Route("")]
    public IActionResult GetPhotoById(int id)
    {
        try
        {
            var photo = _dbContext.Photo.FirstOrDefault(photo => photo.Id == id);
            if (photo == null)
            {
                throw new Exception("Foto não encontrada");
            }

            ////Retornando apenas arquivo de imagem:
            //Byte[] b = System.IO.File.ReadAllBytes(System.IO.Path.Combine(Utils.Utils.GetPublicDir(), photo.FilePath));
            //if(b == null)
            //{
            //    throw new Exception("Foto não encontrada");
            //}
            //return File(b, "image/jpg");

            ////Retornando tudo relacionado à foto:
            //Pegando caminho completo:
            photo.FilePath = System.IO.Path.Combine(Utils.Utils.GetPublicDir(), photo.FilePath);
            //Convertendo de Photo para ReadPhotoDto:
            ReadPhotoDto photoDto = _mapper.Map<ReadPhotoDto>(photo);
            //Retornando:
            return Ok(photoDto);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [HttpGet("user/{userId}")]
    [Authorize]
    //[Route("user")]
    public IActionResult GetUserPhotos(string userId, [FromQuery] int skip = 0, [FromQuery] int take = 9)
    {
        try
        {
            //Verificando se existe usuário com o ID passado:
            User user = _userService.GetUserById(userId);
            if (user == null)
            {
                throw new Exception("Usuário não encontrado");
            }
            //Se existir usuário com esse ID, encontrando todas as fotos deste usuário:
            List<Photo> photos = _dbContext.Photo.Where(photo => photo.UserId == userId).Skip(skip).Take(take).ToList();
            //Se o usuário ainda não tiver nenhuma foto, retornar isso:
            if (photos.Count == 0)
            {
                return Ok("Usuário não possui fotos ainda");
            }
            //Se usuário tiver fotos, pegar caminho completo de cada uma dos fotos:
            foreach (Photo photo in photos)
            {
                photo.FilePath = System.IO.Path.Combine(Utils.Utils.GetPublicDir(), photo.FilePath);
            }
            //Convertendo de uma list de "Photo" para uma de "ReadPhotoDto":
            List<ReadPhotoDto> photosDto = _mapper.Map<List<ReadPhotoDto>>(photos);
            return Ok(photosDto);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("search")]
    public IActionResult SearchPhotos([FromQuery] string title = "")
    {
        try
        {
            //Retornando lista de fotos com base na pesquisa realizada:
            List<Photo> photos = _dbContext.Photo.FromSqlRaw($"SELECT * FROM PHOTO WHERE TITLE LIKE '%{title}%'").ToList();
            if(photos.Count == 0)
            {
                throw new Exception("Nenhum resultado encontrado");
            }

            List<ReadPhotoDto> readPhotoDto = _mapper.Map<List<ReadPhotoDto>>(photos);

            return Ok(readPhotoDto);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{photoId}")]
    public IActionResult UpdatePhotoInfos(int photoId, [FromBody] UpdatePhotoDto updatePhotoDto)
    {
        try
        {
            ////Pegando usuário que está logado:
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            ////Verificando se foto existe:
            var photo = _dbContext.Photo.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null)
            {
                throw new Exception("Foto não encontrada");
            }

            ////Verificando se a foto pertence ao usuário logado:
            if (user.Id != photo.UserId)
            {
                //Retorno se não pertencer:
                throw new Exception("Foto não pertence ao usuário logado");
            }

            //Aplicando alteração na foto:
            _mapper.Map(updatePhotoDto, photo);
            //Salvando alteração no banco de dados:
            _dbContext.SaveChanges();

            //Retornando se tiver dado tudo certo:
            return Ok("Foto atualizada");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [Authorize]
    [HttpDelete("{photoId}")]
    public IActionResult DeletePhoto(int photoId)
    {
        try
        {
            ////Pegando usuário que está logado:
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            ////Verificando se foto existe:
            var photo = _dbContext.Photo.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null)
            {
                throw new Exception("Foto não encontrada");
            }

            ////Verificando se a foto pertence ao usuário logado:
            if (user.Id != photo.UserId)
            {
                //Retorno se não pertencer:
                throw new Exception("Foto não pertence ao usuário logado");
            }

            //Apagando foto:
            _dbContext.Photo.Remove(photo);
            //Salvando alteração no banco:
            _dbContext.SaveChanges();
            //Retornando mensagem ao usuário se deu tudo certo:
            return Ok("Foto apagada");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("comment/{photoId}")]
    public IActionResult GetComentsOfPhoto(int photoId, int skip = 0, int take = 10)
    {
        //Pegando todos os comentários:
        List<ReadCommentDto> commentDto = _mapper.Map<List<ReadCommentDto>>(_dbContext.Comment.FromSqlRaw($"SELECT * FROM COMMENT WHERE PhotoId = {photoId}").Skip(skip).Take(take).ToList());

        if (commentDto.Count == 0)
        {
            return NotFound("Comentários não encontrados");
        }

        return Ok(commentDto);
    }

    [Authorize]
    [HttpPost("comment/{photoId}")]
    public IActionResult AddComment([FromBody] CreateCommentDto commentDto, int photoId)
    {
        try
        {
            ////Pegando usuário que fez o comentário (usuário logado através do token):
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            ////Verificando se foto existe:
            var photo = _dbContext.Photo.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null)
            {
                throw new Exception("Foto não encontrada");
            }

            ////Criando comentário
            //Convertendo de CreateCommentDto para Comment:
            Comment comment = _mapper.Map<Comment>(commentDto);
            //Adicionando ID do usuário ao comentário:
            comment.UserId = user.Id;
            //Adicionando ID da foto ao comentário:
            comment.PhotoId = photoId;
            //Adicionando comentário ao banco de dados:
            _dbContext.Comment.Add(comment);
            //Salvando alteração no banco de dados:
            _dbContext.SaveChanges();

            return Ok("Comentário adicionado");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [Authorize]
    [HttpPut("comment/{commentId}")]
    public IActionResult UpdateComment(UpdateCommentDto commentDto, int commentId)
    {
        try
        {
            ////Pegando usuário que quer editar o comentário (usuário logado através do token):
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            //Encontrando comentário pelo ID passado via params na requisição:
            var comment = _dbContext.Comment.FirstOrDefault(comment => comment.Id == commentId);
            //Retornando se não tiver encontrado nenhum comentário:
            if (comment == null)
            {
                throw new Exception("Comentário não encontrado");
            }

            //Verificando se comentário não pertence ao usuário logado:
            if (user.Id != comment.UserId)
            {
                //Retorno se não pertencer:
                throw new Exception("Comentário não pertence ao usuário logado");
            }

            //Se pertencer ao usuário logado, então aplicar as alterações ao objeto salvo no banco:
            _mapper.Map(commentDto, comment);
            //E salvar a alteração:
            _dbContext.SaveChanges();
            //Retornando mensagem ao usuário
            return Ok("Comentário editado");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [Authorize]
    [HttpDelete("comment/{commentId}")]
    public IActionResult DeleteComment(int commentId)
    {
        try
        {
            ////Pegando usuário que fez o comentário (usuário logado através do token):
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            //Encontrando comentário pelo ID passado via params na requisição:
            var comment = _dbContext.Comment.FirstOrDefault(comment => comment.Id == commentId);
            //Retornando se não tiver encontrado nenhum comentário:
            if (comment == null)
            {
                throw new Exception("Comentário não encontrado");
            }

            //Verificando se comentário não pertence ao usuário logado:
            if (user.Id != comment.UserId)
            {
                //Retorno se não pertencer:
                throw new Exception("Comentário não pertence ao usuário logado");

            }

            //Se pertencer ao usuário logado, então deleta-lo do banco:
            _dbContext.Comment.Remove(comment);
            //E salvar a alteração:
            _dbContext.SaveChanges();
            return Ok("Comentário apagado");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [Authorize]
    [HttpPost("like/{photoId}")]
    public IActionResult AddLike(int photoId)
    {
        try
        {
            //Aqui não precisamos de payload no body, pois é um MODELO baseado apenas em IDs

            ////Pegando usuário que deu o like (usuário logado através do token):
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            ////Verificando se foto existe:
            var photo = _dbContext.Photo.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null)
            {
                throw new Exception("Foto não encontrada");
            }

            ////Verificando se usuário não está curtindo a foto pela segunda vez:
            var likeCheck = _dbContext.Like.FirstOrDefault(like => like.UserId == user.Id && like.PhotoId == photoId);
            if (likeCheck != null)
            {
                throw new Exception("O usuário não pode curtir a mesma foto duas vezes");
            }

            //Inicializando objeto like vazio:
            Like like = new Like();
            //Adicionando ID do usuário ao Like:
            like.UserId = user.Id;
            //Adicionando ID da foto ao comentário:
            like.PhotoId = photoId;
            //Adicionando comentário ao banco de dados:
            _dbContext.Like.Add(like);
            //Salvando alteração no banco de dados:
            _dbContext.SaveChanges();

            return Ok("Like adicionado");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [Authorize]
    [HttpDelete("like/{likeId}")]
    public IActionResult DeleteLike(int likeId)
    {
        try
        {
            ////Pegando usuário que fez o comentário (usuário logado através do token):
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new Exception("Error");
            }
            User user = _userService.GetCurrentUser(identity);

            //Encontrando comentário pelo ID passado via params na requisição:
            var like = _dbContext.Like.FirstOrDefault(like => like.Id == likeId);
            //Retornando se não tiver encontrado nenhum comentário:
            if (like == null)
            {
                throw new Exception("Like não encontrado");
            }

            //Verificando se comentário não pertence ao usuário logado:
            if (user.Id != like.UserId)
            {
                //Retorno se não pertencer:
                throw new Exception("Like não pertence ao usuário logado");

            }

            //Se pertencer ao usuário logado, então deleta-lo do banco:
            _dbContext.Like.Remove(like);
            //E salvar a alteração:
            _dbContext.SaveChanges();
            return Ok("Like apagado");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("like/{photoId}")]
    public IActionResult GetLikesOfPhoto(int photoId)
    {
        //Pegando todos os likes:
        List<ReadLikeDto> likeDto = _mapper.Map<List<ReadLikeDto>>(_dbContext.Like.FromSqlRaw($"SELECT * FROM [LIKE] WHERE PhotoId = {photoId}").ToList());

        if (likeDto.Count == 0)
        {
            return NotFound("Essa foto ainda não possui curtidas");
        }

        return Ok(likeDto);
    }


}
