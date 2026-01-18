create table chat.sys_user
(
	id						serial not null primary key,
	user_name				varchar(250) not null,
	password_hash			varchar(250) not null,
	password_salt			varchar(250) not null,
	full_name 			    varchar(250) not null,
	phone_number			varchar(50),
	state_id				int not null,
	created_at			    timestamp without time zone default now() not null,
	created_user_id			int null,
	modified_at		        timestamp without time zone,
	modified_user_id		int,

	constraint fk_state_id					foreign key ( state_id )				references chat.enum_state ( id )
);

create unique index sys_user_unique_index_user_name on chat.sys_user (user_name);
