create table chat.enum_state
(id          int not null primary key,
 order_code  varchar(50) null,
 short_name  varchar(250) not null,
 full_name   varchar(250) not null,
 created_at  timestamp without time zone default now() not null
);

insert into chat.enum_state (id, short_name, full_name) 
VALUES (1,N'А',N'Актив'),(2,N'П',N'Пассив');
