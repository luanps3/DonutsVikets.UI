------------------------------------------------------------
--  CRIAR BANCO DE DADOS
------------------------------------------------------------
IF DB_ID('DonutsVikets3Context') IS NOT NULL
    DROP DATABASE DonutsVikets3Context;
GO

CREATE DATABASE DonutsVikets3Context;
GO

USE DonutsVikets3Context;
GO


------------------------------------------------------------
--  TABELA: TipoUsuario
------------------------------------------------------------
CREATE TABLE TipoUsuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL
);
GO


------------------------------------------------------------
--  TABELA: Usuario
--  Model: Usuario.cs
------------------------------------------------------------
CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    SenhaHash NVARCHAR(500) NOT NULL,
    TipoUsuarioId INT NOT NULL,
    CONSTRAINT FK_Usuario_TipoUsuario FOREIGN KEY (TipoUsuarioId)
        REFERENCES TipoUsuario(Id)
);
GO


------------------------------------------------------------
--  TABELA: Categoria
------------------------------------------------------------
CREATE TABLE Categoria (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL
);
GO


------------------------------------------------------------
--  TABELA: Produto
--  Model: Produto.cs
------------------------------------------------------------
CREATE TABLE Produto (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(200) NOT NULL,
    Preco DECIMAL(18,2) NOT NULL,
    CategoriaId INT NOT NULL,
    Imagem NVARCHAR(MAX) NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Produto_Categoria FOREIGN KEY (CategoriaId)
        REFERENCES Categoria(Id)
);
GO


------------------------------------------------------------
--  TABELA: Pedido
--  Model: Pedido.cs
------------------------------------------------------------
CREATE TABLE Pedido (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    DataPedido DATETIME NOT NULL,
    CONSTRAINT FK_Pedido_Usuario FOREIGN KEY (UsuarioId)
        REFERENCES Usuario(Id)
);
GO


------------------------------------------------------------
--  TABELA: ItemPedido
--  Model: ItemPedido.cs
------------------------------------------------------------
CREATE TABLE ItemPedido (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PedidoId INT NOT NULL,
    ProdutoId INT NOT NULL,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(18,2) NOT NULL,

    CONSTRAINT FK_ItemPedido_Pedido FOREIGN KEY (PedidoId)
        REFERENCES Pedido(Id),

    CONSTRAINT FK_ItemPedido_Produto FOREIGN KEY (ProdutoId)
        REFERENCES Produto(Id)
);
GO


------------------------------------------------------------
--  INSERTS INICIAIS (SEED)
------------------------------------------------------------

---------------------------
-- Tipos de Usuário
---------------------------
INSERT INTO TipoUsuario (Nome) VALUES 
('Administrador'),
('Cliente');
GO


---------------------------
-- Usuário padrão
-- SenhaHash = SHA256('123456')
---------------------------
INSERT INTO Usuario (Nome, Email, SenhaHash, TipoUsuarioId)
VALUES (
    'Admin',
    'admin@teste.com',
    '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', 
    1
);
GO


---------------------------
-- Categorias
---------------------------
INSERT INTO Categoria (Nome) VALUES
('Clássicos'),
('Recheados'),
('Premium');
GO


---------------------------
-- Produtos
---------------------------
INSERT INTO Produto (Nome, Preco, CategoriaId, Imagem, Ativo)
VALUES
('Donut Tradicional', 5.50, 1, NULL, 1),
('Donut Recheado Chocolate', 8.90, 2, NULL, 1),
('Donut Caramelo Premium', 12.50, 3, NULL, 1);
GO


---------------------------
-- Pedido exemplo
---------------------------
INSERT INTO Pedido (UsuarioId, DataPedido)
VALUES (1, GETDATE());
GO

---------------------------
-- Itens do Pedido exemplo
---------------------------
INSERT INTO ItemPedido (PedidoId, ProdutoId, Quantidade, PrecoUnitario)
VALUES
(1, 1, 2, 5.50),
(1, 2, 1, 8.90);
GO


------------------------------------------------------------
-- FIM DO SCRIPT
------------------------------------------------------------
