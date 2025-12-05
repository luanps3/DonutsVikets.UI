using DonutsVikets.DAL.Repositories;
using DonutsVikets.DTOs;
using Microsoft.EntityFrameworkCore;
using Project.BLL.Services;
using Project.DAL.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonutsVikets.UI
{
    public partial class frmUsuarios2 : Form
    {
        private DonutsVikets3Context _context;
        private UsuarioRepository _usuarioRepo;
        private UsuarioService _usuarioService;

        public frmUsuarios2()
        {
            InitializeComponent();
            SetupDependencies();
        }

        private void SetupDependencies()
        {
            var options = new DbContextOptionsBuilder<DonutsVikets3Context>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=DonutsVikets3Context;Trusted_Connection=True;")
                .Options;

            _context = new DonutsVikets3Context(options);
            _context.Database.EnsureCreated();

            _usuarioRepo = new UsuarioRepository(_context);
            _usuarioService = new UsuarioService(_usuarioRepo);
        }

        private async void frmUsuarios2_Load(object sender, EventArgs e)
        {
            await CarregarTiposUsuarioAsync();
            await CarregarGridAsync();
        }

        private async Task CarregarTiposUsuarioAsync()
        {
            var tipos = await _context.TipoUsuarios.AsNoTracking().ToListAsync();
            cmbTipoUsuario.DataSource = tipos;
            cmbTipoUsuario.DisplayMember = "Nome";
            cmbTipoUsuario.ValueMember = "Id";
        }

        private async Task CarregarGridAsync()
        {
            var lista = (await _usuarioService.GetAllAsync()).ToList();
            dgvUsuarios.DataSource = lista;

            // Ajustes: esconder senha se houver
            if (dgvUsuarios.Columns.Cast<DataGridViewColumn>().Any(c => c.Name == "Senha"))
                dgvUsuarios.Columns["Senha"].Visible = false;

            if (dgvUsuarios.Columns.Cast<DataGridViewColumn>().Any(c => c.Name == "TipoUsuarioId"))
                dgvUsuarios.Columns["TipoUsuarioId"].Visible = false;

            dgvUsuarios.AutoResizeColumns();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;
            var dto = dgvUsuarios.CurrentRow.DataBoundItem as UsuarioDTO;
            if (dto == null) return;

            txtId.Text = dto.Id.ToString();
            txtNome.Text = dto.Nome;
            txtEmail.Text = dto.Email;
            txtSenha.Text = string.Empty;
            if (dto.TipoUsuarioId != 0)
                cmbTipoUsuario.SelectedValue = dto.TipoUsuarioId;
        }

        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            await CarregarGridAsync();

        }

        private async void btnAtualizar_Click_1(object sender, EventArgs e)
        {
            await CarregarGridAsync();
        }

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
                Senha = txtSenha.Text, // se vazio, não altera
                TipoUsuarioId = Convert.ToInt32(cmbTipoUsuario.SelectedValue)
            };

            var res = await _usuarioService.UpdateAsync(dto);
            MessageBox.Show(res.message);
            if (res.success) await CarregarGridAsync();
        }

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
}
