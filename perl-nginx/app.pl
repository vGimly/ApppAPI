#!/usr/bin/perl

use Mojolicious::Lite -signatures;
use Mojo::mysql;

#use DBI;DBI->trace(1,'log/dbitrace.log');$DBI::neat_maxlen=4096;

my $cfg = plugin JSONConfig => {file => './app.conf'};
app->secrets(delete $cfg->{secrets});
app->log->level($cfg->{log}//'info');

my $mysql_cfg=delete $cfg->{mysql};
helper mysql => sub { state $mysql = Mojo::mysql->strict_mode($mysql_cfg) };

sub CatchError {my($c,$err)=@_;$c->log->error($err);$c->render(text=>$err,format=>'txt',code=>500)}

get '/' => sub ($c) { $c->render(json => $c->mysql->db->query('select now() as time')->hash) };

get '/usluga' => sub ($c) {
	$c->mysql->db->query_p('SELECT `usluga_id` as UslugaId,`usluga_name` as UslugaName FROM `app`.`usluga`')
	 ->then(sub{$c->render(json=>shift->hashes)})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

post '/usluga' => sub ($c) { my $p = $c->req->json;
	$c->mysql->db->query_p('INSERT INTO `app`.`usluga` (`usluga_name`) VALUES (?) ON DUPLICATE KEY UPDATE `usluga_id`=LAST_INSERT_ID(`usluga_id`)',
		$p->{UslugaName})
	 ->then(sub{$c->render(json=>{ok=>shift->last_insert_id})})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

# rename
put '/usluga/<:usluga-id>' => sub ($c) { my $p = $c->req->json;
	if ($p->{UslugaId} ne $c->param('usluga-id')) {return $c->render(text=>'Wrong UslugaId',code=>405)}
	$c->mysql->db->query_p('UPDATE `app`.`usluga` SET `usluga_name`=? WHERE `usluga_id`=?'
		,@$p{qw/UslugaName UslugaId/})
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

del '/usluga/<:usluga-id>' => sub ($c) {
	$c->mysql->db->query_p('DELETE FROM `app`.`usluga` WHERE `usluga_id`=?'
		,$c->param('usluga-id'))
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

get '/usluga/<:usluga-id>/tariff' => sub ($c) {
	my $uid=$c->param('usluga-id');
	$uid=undef if $uid eq 'null' or $uid eq 'undefined';
	my @params;push @params,$uid if defined $uid;
	$c->mysql->db->query_p('SELECT `tariff_id` as TariffId,`usluga_ref` as UslugaRef,`t_date` as TDate,`price` as Price FROM `app`.`tariff`'
	    .(defined($uid)?' WHERE `usluga_ref`=?':'').' ORDER BY `t_date` ASC'
	    ,@params)
	 ->then(sub{$c->render(json=>shift->hashes)})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

# add
post '/usluga/<:usluga-id>/tariff' => sub ($c) { my $p = $c->req->json;
	if ($p->{UslugaId} ne $c->param('usluga-id')) {return $c->render(text=>'Wrong UslugaId',code=>405)}
	$c->mysql->db->query_p('INSERT INTO `app`.`tariff` (`usluga_ref`,`t_date`,`price`) VALUES (?,?,?)'
		,@$p{qw/UslugaRef TariffStart TariffValue/})
	 ->then(sub{$c->render(json=>{OK=>shift->last_insert_id})})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

put '/tariff/<:tariff-id>' => sub ($c) { my $p = $c->req->json;
	if ($p->{TariffId} ne $c->param('tariff-id')) {return $c->render(text=>'Wrong TariffId',code=>405)}
	$c->mysql->db->query_p('UPDATE `app`.`tariff` SET `t_date`=?,`price`=? WHERE `tariff_id`=?'
		,@$p{qw/TariffStart TariffValue TariffId/})
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

del '/tariff/<:tariff-id>' => sub ($c) {
	$c->mysql->db->query_p('DELETE FROM `app`.`tariff` WHERE `tariff_id`=?'
		,$c->param('tariff-id'))
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

get '/usluga/<:usluga-id>/counter' => sub ($c) {
	my $uid=$c->param('usluga-id');
	$uid=undef if $uid eq 'null' or $uid eq 'undefined';
	my @params;push @params,$uid if defined $uid;
#	$c->mysql->db->query_p('SELECT `counter_id`,`usluga_ref`,`counter_name`,`serial`,`digits`,`precise` FROM `app`.`counters`'
	$c->mysql->db->query_p('SELECT `counter_id` as CounterId,`usluga_ref` as UslugaRef,`counter_name` as CounterName,'
	    .'`serial` as Serial,`digits` as Digits,`precise` as Precise,'
	    .'`start_date` as StartDate,`init_value` as InitValue FROM `app`.`counter_with_initials`'
	.(defined($uid)?' WHERE `usluga_ref`=?':'').' ORDER BY counter_name,serial',@params)
	 ->then(sub{$c->render(json=>shift->hashes)})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

get '/usluga/<:usluga-id>/tariff/<:date>' => sub ($c) {
	$c->mysql->db->query_p('SELECT price FROM tariff t WHERE t.usluga_ref=? AND t_date<=? ORDER BY t_date desc LIMIT 1'
		,$c->param('usluga-id'),$c->param('date'))
	 ->then(sub{
		my $res=shift;
		if ($res->rows) {$res=$res->array->[0]}
		else {$res=''}
		$c->render(text=>$res,format=>'txt');
	})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

post '/usluga/<:usluga-id>/counter' => sub ($c) { my $p = $c->req->json;
	if ($p->{UslugaId} ne $c->param('usluga-id')) {return $c->render(text=>'Wrong UslugaId',code=>405)}
	my $cnt_id;my $db=$c->mysql->db;
	$db->query_p('INSERT INTO `app`.`counters` (`usluga_ref`,`counter_name`,`serial`,`digits`,`precise`) VALUES (?,?,?,?,?)'
		,@$p{qw/UslugaId CounterName Serial Digits Precise/})
	 ->then(sub{$cnt_id=shift->last_insert_id;
	    return $db->query_p('INSERT INTO `app`.`measures` (`counter_ref`,`m_date`,`value`) VALUES (?,?,?)', $cnt_id
		,@$p{qw/StartDate InitValue/})})
	 ->then(sub{$c->render(json=>{OK=>$cnt_id, MEA=>shift->last_insert_id})})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

put '/counter/<:counter-id>' => sub ($c) { my $p = $c->req->json;
	if ($p->{CounterId} ne $c->param('counter-id')) {return $c->render(text=>'Wrong CounterId',code=>405)}
	$c->mysql->db->query_p('UPDATE `app`.`counters` SET `counter_name`=?,`serial`=?,`digits`=?,`precise`=? WHERE `counter_id`=?'
		,@$p{qw/UslugaId CounterName Serial Digits Precise CounterId/})
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

put '/measure/<:m_id>' => sub ($c) { my $p = $c->req->json;
	if ($p->{MId} ne $c->param('m_id')) {return $c->render(text=>'Wrong MId',code=>405)}
	$c->mysql->db->query_p('UPDATE `app`.`measures` SET `m_date`=?,`value`=? WHERE `m_id`=?'
		,@$p{qw'MDate Value'},$c->param('m_id'))
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

post '/counter/<:counter-id>/measure' => sub ($c) { my $p = $c->req->json;
	$c->mysql->db->query_p('INSERT INTO `app`.`measures` (`counter_ref`,`m_date`,`value`) VALUES (?,?,?)'
		,$c->param('counter-id'),@$p{qw/MDate Value/})
	 ->then(sub{$c->render(json=>{OK=>shift->last_insert_id})})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

del '/measure/<:m_id>' => sub ($c) {
	$c->mysql->db->query_p('DELETE FROM `app`.`measures` WHERE `m_id`=?'
		,$c->param('m_id'))
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

del '/counter/<:counter-id>' => sub ($c) {
	$c->mysql->db->query_p('DELETE FROM `app`.`counters` WHERE `counter_id`=?'
		,$c->param('counter-id'))
	 ->then(sub{$c->render(text=>'OK'.(shift->affected_rows),format=>'txt')})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

get '/counter/<:counter-id>/measure' => sub ($c) {
	my $uid=$c->param('counter-id');
	$uid=undef if $uid eq 'null' or $uid eq 'undefined';
	my @params;push @params,$uid if defined $uid;

	$c->mysql->db->query_p(<<EOF . (defined($uid)?' WHERE counter_id=?' :'') . ' ORDER BY m_date asc' , @params)
SELECT counter_id as CounterRef,m_id as MId,m_date as MDate,`value` as Value,digits as Digits,precise as Precise,init_value as InitValue,
(SELECT price FROM tariff t WHERE t.usluga_ref=c.usluga_ref AND t_date<=m_date order BY t_date desc LIMIT 1) AS Tariff
FROM measures m INNER JOIN counter_with_initials c ON counter_ref=counter_id
EOF
	 ->then(sub{$c->render(json=>shift->hashes)})
	 ->catch(sub{CatchError($c,@_)});
	$c->render_later;
};

app->start;
