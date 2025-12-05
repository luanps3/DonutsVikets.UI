// Project.UI/Forms/frmUsuarios.cs (trechos)
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project.BLL.Services;
using Project.DAL.Data;
using Project.DAL.Repositories;
using DonutsVikets.Models;
using DonutsVikets.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public partial class frmUsuarios : Form
{
    private UsuarioService _usuarioService;
    private DonutsVikets3Context _context;

    public frmUsuarios()
    {
        InitializeComponent();
        SetupDependencies();
    }

    private void SetupDependencies()
    {
        // Criar Options e instanciar repositório/service localmente (sem DI container)
        var options = new DbContextOptionsBuilder<DonutsVikets3Context>()
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DonutsVikets3Context;Trusted_Connection=True;")
            .Options;

        _context = new DonutsVikets3Context(options);
        // opcional: criar DB se não existir
        _context.Database.EnsureCreated();

        var repo = new UsuarioRepository(_context);
        _usuarioService = new UsuarioService(repo);
    }

    // Chamar no Load do Form
    private async void frmUsuarios_Load(object sender, EventArgs e)
    {
        await CarregarTiposUsuarioAsync();
        await CarregarGridAsync();
    }

    private async Task CarregarTiposUsuarioAsync()
    {
        // Carregar tipos (para o combobox)
        var tipos = await _context.TipoUsuarios.AsNoTracking().ToListAsync();
        cmbTipoUsuario.DataSource = tipos;
        cmbTipoUsuario.DisplayMember = "Nome";
        cmbTipoUsuario.ValueMember = "Id";
    }

    private async Task CarregarGridAsync()
    {
        var lista = (await _usuarioService.GetAllAsync()).ToList();
        dgvUsuarios.DataSource = lista;

        // Ajustes visuais
        dgvUsuarios.Columns["Senha"].Visible = false;
        dgvUsuarios.Columns["TipoUsuarioId"].Visible = false;
        dgvUsuarios.AutoResizeColumns();
    }

    // Ao selecionar linha, preencher campos
    private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvUsuarios.CurrentRow == null) return;
        var dto = dgvUsuarios.CurrentRow.DataBoundItem as UsuarioDTO;
        if (dto == null) return;

        txtId.Text = dto.Id.ToString();
        txtNome.Text = dto.Nome;
        txtEmail.Text = dto.Email;
        txtSenha.Text = string.Empty; // não exibir hash
        if (dto.TipoUsuarioId != 0)
            cmbTipoUsuario.SelectedValue = dto.TipoUsuarioId;
    }

    // Botão Atualizar
    private async void btnAtualizar_Click(object sender, EventArgs e)
    {
        await CarregarGridAsync();
    }

    // Botão Adicionar
    private async void btnAdicionar_Click(object sender, EventArgs e)
    {
        var dto = new UsuarioDTO
        {
            Nome = txtNome.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Senha = txtSenha.Text,
            TipoUsuarioId = Convert.ToInt32(cmbTipoUsuario.SelectedValue)
        };

        var res = await _usuarioService.CreateAsync(dto);
        MessageBox.Show(res.message);
        if (res.success) await CarregarGridAsync();
    }

    // Botão Editar
    private async void btnEditar_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtId.Text, out var id))
        {
            MessageBox.Show("Selecione um usuário válido.");
            return;
        }

        var dto = new UsuarioDTO
        {
            Id = id,
            Nome = txtNome.Text.Trim(),
            Email = txtEmail.Text.Trim(),
            Senha = txtSenha.Text, // se vazio, BLL não altera
            TipoUsuarioId = Convert.ToInt32(cmbTipoUsuario.SelectedValue)
        };

        var res = await _usuarioService.UpdateAsync(dto);
        MessageBox.Show(res.message);
        if (res.success) await CarregarGridAsync();
    }

    // Botão Excluir
    private async void btnExcluir_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtId.Text, out var id))
        {
            MessageBox.Show("Selecione um usuário válido.");
            return;
        }

        var confirm = MessageBox.Show("Confirma exclusão?", "Excluir", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        var res = await _usuarioService.DeleteAsync(id);
        MessageBox.Show(res.message);
        if (res.success) await CarregarGridAsync();
    }
}
