create table chat.hl_message
(
	id					bigserial 	   NOT NULL PRIMARY KEY,		
	sender_user_id	    integer        NOT NULL REFERENCES chat.sys_user,
	reciever_user_id	integer        NOT NULL REFERENCES chat.sys_user,
	message_text		text			NOT NULL,
	is_pinned			bool            NOT NULL DEFAULT false,
	is_read				bool            NOT NULL,	
	created_at		    timestamp without time zone DEFAULT NOW() NOT NULL,
	created_user_id		int,
	modified_at		    timestamp without time zone,
	modified_user_id	int
);

create index idx_message_pair
on chat.hl_message (
    least(sender_user_id, reciever_user_id),
    greatest(sender_user_id, reciever_user_id),
    created_at desc
);

create index idx_message_unread on chat.hl_message (reciever_user_id, is_read);
