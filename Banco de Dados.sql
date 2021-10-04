create database "IoniCRM"
    with
    owner = postgres
    encoding = 'UTF8'
    lc_collate = 'Portuguese_Brazil.1252'
    lc_ctype = 'Portuguese_Brazil.1252'
    tablespace = pg_default
    connection limit = -1;

create table Usuario(
	pk_usuario serial primary key,
	nivel integer not null,
	nome varchar(80) not null,
	email varchar(100) unique not null,
	hash_senha varchar(50) not null,
	cargo varchar(40)
);

create table Cliente(
	pk_cliente serial primary key,
	fk_emp integer,
	cpf_cnpj varchar(20) not null unique,
	crm varchar(15),
	nome varchar(80) not null,
	razao_social varchar(250) unique, /*ou apelido*/
	categoria varchar(40), /*ou cargo*/
	descr varchar(200),
	foreign key (fk_emp) references Cliente (pk_cliente)
);

create table Cliente_Info(
	pk_info serial primary key,
	fk_cliente integer not null,
	endereco varchar(200),
	website varchar(50),
	foreign key (fk_cliente) references Cliente (pk_cliente)
);

create table Cliente_Contato(
	pk_contato serial primary key,
	fk_cliente integer not null,
	tipo integer,
	contato varchar(80),
	foreign key (fk_cliente) references Cliente (pk_cliente)
);

create table Pipeline(
	pk_pipeline serial primary key,
	nome varchar(80) not null,
	prioridade integer default 0,
	descr varchar(200)
);

create table Deal_Info(
	pk_df serial primary key,
	abertura timestamp,
	fechamento timestamp,
	probabilidade integer default 100,
	descr varchar(200)
);

create table Deal(
	pk_deal serial primary key,
	fk_pipeline integer not null,
	fk_df integer not null,
	nome varchar(80) not null,
	estagio integer default 0, /*em que estagio da pipeline ele está*/
	/*d_status:
		0 - Open
		1 - Won
		2 - Lost
	*/
	d_status integer default 0, 
	valor numeric(7,2) default 0.00,
	foreign key (fk_pipeline) references Pipeline (pk_pipeline),
	foreign key (fk_df) references Deal_Info (pk_df)
);

create table Deal_Cliente(
	pk_dc serial primary key,
	fk_deal integer not null,
	fk_cliente integer not null,
	foreign key (fk_deal) references Deal (pk_deal),
	foreign key (fk_cliente) references Cliente (pk_cliente)
);

create table Usuario_Pipeline(
	pk_sp serial primary key,
	fk_usuario integer not null,
	fk_pipeline integer not null,
	foreign key (fk_usuario) references Usuario (pk_usuario),
	foreign key (fk_pipeline) references Pipeline (pk_pipeline)
);

/**/

/*
select login(<email> varchar, <hash_senha> varchar);
*/
create or replace function login(email_x varchar, hash_senha_x varchar)
returns boolean
language plpgsql
as $$
declare
	login_x bigint := (
		select count(pk_usuario) from Usuario
			where 
				email like email_x
			and
				hash_senha like hash_senha_x);
begin
	case when login_x > 0 
		then return 1;
		else return 0;
	end case;
end $$;

/*
select addUsuario(<dados> json);
*/
create or replace function addUsuario(dados json)
returns void
language plpgsql
as $$
begin
	insert into Usuario values
	(
		default,
		(dados->>'nivel')::integer,
		dados->>'nome',
		dados->>'email',
		dados->>'hash_senha',
		dados->>'cargo'
	)
	on conflict (email) do update
	set
		nivel = excluded.nivel,
		nome = excluded.nome,
		hash_senha = excluded.hash_senha,
		cargo = excluded.cargo;
		
end $$;

/*
select * from dadosCliente(<pk_cliente> integer);
*/
create or replace function dadosCliente(id_cliente integer)
returns table(
	emp varchar,
	nome varchar,
	cpf_cnpj varchar,
	crm varchar,
	razao_social varchar,
	categoria varchar,
	descr varchar,
	website varchar,
	endereco varchar,
	tipo_contato varchar,
	contato varchar
)
language plpgsql
as $$
begin
	return query
	with cli_x as (
		select * from Cliente
		), 

		cli_info as (
			select 
				fk_cliente as fk,
				string_agg(Cliente_Info.website,';' order by Cliente_Info.endereco)::varchar as website,
				string_agg(Cliente_Info.endereco,';' order by Cliente_Info.endereco)::varchar as endereco
			from Cliente_Info
			group by fk_cliente
		),

		cli_contato as (
			select 
				fk_cliente as fk,
				string_agg(Cliente_Contato.tipo::text,';' order by tipo)::varchar as tipo_contato,
				string_agg(Cliente_Contato.contato,';' order by tipo)::varchar as contato
			from Cliente_Contato
			group by fk_cliente
		),

		emp_x as (
			select Cliente.pk_cliente as pk_emp, Cliente.razao_social as nome from Cliente
			where fk_emp is null
		)

	select
		(
			case when cli_x.nome not like emp_x.nome
				then emp_x.nome
				else null
			end
		),
		cli_x.nome,
		cli_x.cpf_cnpj,
		cli_x.crm,
		cli_x.razao_social,
		cli_x.categoria,
		cli_x.descr,
		cli_info.website,
		cli_info.endereco,
		cli_contato.tipo_contato,
		cli_contato.contato

	from 
		cli_x, 
		emp_x,
		cli_info,
		cli_contato

	where 
		cli_x.pk_cliente = id_cliente
	and cli_x.pk_cliente = cli_info.fk
	and cli_info.fk = cli_contato.fk
	and emp_x.pk_emp = (
			case when cli_x.fk_emp is null
			then cli_x.pk_cliente
			else cli_x.fk_emp
			end
		);

end $$;


/*
select addCliente(<dados> json);
*/
create or replace function addCliente(dados json)
returns void
language plpgsql
as $$
begin
	insert into Cliente values
	(
		default,
		(dados->>'fk_emp')::integer,
		dados->>'cpf_cnpj',
		dados->>'crm',
		dados->>'nome',
		dados->>'razao_social',
		dados->>'categoria',
		dados->>'descr'
	)
	on conflict (cpf_cnpj) do update
	set
		fk_emp = excluded.fk_emp,
		crm = excluded.crm,
		nome = excluded.nome,
		razao_social = excluded.razao_social,
		categoria = excluded.categoria,
		descr = excluded.descr;
end $$;

/*
select atualizarCPF_CNPJ(<antigo> varchar, <novo> varchar);
*/
create or replace function atualizarCPF_CNPJ(antigo varchar, novo varchar)
returns void
language plpgsql
as $$
begin
	update Cliente
		set cpf_cnpj = novo
		where cpf_cnpj = antigo;
end $$;


/*
select addClienteInfo(<dados> json);
*/
create or replace function addClienteInfo(dados json)
returns void
language plpgsql
as $$
begin
	insert into Cliente_Info values
	(
		default,
		(dados->>'fk_cliente')::integer,
		dados->>'endereco',
		dados->>'website'
	);
end $$;


/*
select updateClienteInfo(<dados> json);
*/
create or replace function updateClienteInfo(id_info integer, dados json)
returns void
language plpgsql
as $$
begin
	update Cliente_Info
	set
		fk_cliente = (dados->>'fk_cliente')::integer,
		endereco = dados->>'endereco',
		website = dados->>'website'
	where pk_info = id_info;
end $$;


/*
select addClienteContato(<dados> json);
*/
create or replace function addClienteContato(dados json)
returns void
language plpgsql
as $$
begin
	insert into Cliente_Contato values
	(
		default,
		(dados->>'fk_cliente')::integer,
		(dados->>'tipo')::integer,
		dados->>'contato'
	);
end $$;


/*
select updateClienteContato(<dados> json);
*/
create or replace function updateClienteContato(id_contato integer, dados json)
returns void
language plpgsql
as $$
begin
	update Cliente_Contato
	set
		fk_cliente = (dados->>'fk_cliente')::integer,
		tipo = (dados->>'tipo')::integer,
		contato = dados->>'contato'
	where pk_info = id_info;
end $$;


/*----------------*/

create or replace function getDeal(id_deal integer)
returns table(
	pipeline varchar,
	nome varchar,
	estagio integer,
	d_status integer,
	valor numeric,
	cliente varchar,
	apelido varchar,
	probabilidade integer,
	abertura timestamp,
	fechamento timestamp,
	descr varchar
)
language plpgsql
as $$
begin
	return query with 
		deal_x as (
			select * from Deal
		),
	
		dinfo as (
			select * from Deal_Info
		),
		
		cli as (
			select Cliente.nome, Cliente.apelido, Deal_Cliente.fk_deal
				from Cliente, Deal_Cliente
				where Deal_Cliente.fk_cliente = Cliente.pk_cliente
		),
		
		pipe as (
			select nome from Pipeline
		)
		
		select
			pipe.nome,
			deal_x.nome,
			deal_x.estagio,
			deal_x.d_status,
			deal_x.valor,
			cli.nome,
			cli.apelido,
			dinfo.probabilidade,
			dinfo.abertura,
			dinfo.fechamento,
			deal_x.descr
		
		from
			deal_x,
			dinfo,
			cli,
			pipe
		
		where
			deal_x.pk_deal = id_deal
		and deal_x.fk_pipeline = pipe.pk_pipeline
		and deal_x.fk_df = dinfo.pk_df
		and deal_x.pk_deal = cli.fk_deal;
	
end $$;


create or replace function addDeal(dados json)
returns void
language plpgsql
as $$
declare
	id_cli integer := (select pk_cliente from Cliente
		where apelido = dados->>'apelido');
	deal_open timestamp := to_timestamp(dados->>'abertura', 'MM/DD/YYYY HH24:MI:SS');
	deal_close timestamp := to_timestamp(dados->>'fechamento', 'MM/DD/YYYY HH24:MI:SS');
begin
	with dinfo as (
		insert into Deal_Info values
		(
			default,
			deal_open,
			deal_close,
			dados->>'probabilidade',
			dados->>'descr'
		)
		returning pk_df
	),
	
	pipe as (
		select pk_pipeline from Pipeline
			where nome = dados->>'pipeline'
	),
	
	deal_x as (
		insert into Deal values
		(
			default,
			pipe.pk_pipeline,
			dinfo.pk_df,
			dados->>'nome',
			(dados->>'estagio')::integer,
			(dados->>'d_status')::integer,
			(dados->>'valor')::numeric
		)
		returning pk_deal
	)
		
	insert into Deal_Cliente values
	(
		default,
		deal_x.pk_deal,
		id_cli
	);
	
end $$;

create or replace function updateDeal(dados json)
returns void
language plpgsql
as $$
declare
	id_cli integer := (select pk_cliente from Cliente
		where apelido = dados->>'apelido');
	deal_open timestamp := to_timestamp(dados->>'abertura', 'MM/DD/YYYY HH24:MI:SS');
	deal_close timestamp := to_timestamp(dados->>'fechamento', 'MM/DD/YYYY HH24:MI:SS');
begin
	with dinfo as (
		update Deal_Info 
		set
			abertura = deal_open,
			fechamento = deal_close,
			probabilidade = dados->>'probabilidade',
			descr = dados->>'descr'
		where pk_df = (dados->>'fk_df')::integer
		returning pk_df
	),
	
	pipe as (
		select pk_pipeline from Pipeline
			where nome = dados->>'pipeline'
	),
	
	deal_x as (
		update Deal 
		set
			fk_pipeline = pipe.pk_pipeline,
			fk_df = dinfo.pk_df,
			nome = dados->>'nome',
			estagio = (dados->>'estagio')::integer,
			d_status = (dados->>'d_status')::integer,
			valor = (dados->>'valor')::numeric
		where pk_deal = dados->>'id_deal'
		returning pk_deal
	)
		
	update Deal_Cliente 
		set fk_cliente = id_cli
		where fk_deal = deal_x.pk_deal;
	
end $$;











