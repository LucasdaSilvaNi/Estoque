$(document).ready(function(){
    $('#container_abas_tabelas').prepend('<ul id="nav"><li><a href="#" class="corrente">Estrutura Organizacional</a></li><li><a href="#">Cat&aacute;logo</a></li><li><a href="#">Outras</a></li></ul>')
  
  $('.aba').css({
    display: 'none',
    marginTop: 0,
    borderTopWidth: 0
    })
    
  $('#aba1').css('display', 'block');
  $('a', $('#nav')).click(function() {    
	var i = $('a', $('#nav')).index(this) + 1;      
	  $(this).parents('#container_abas_tabelas')
	  .children('.aba:visible').hide();      
	  $('#aba' + i).show();      
	  $(this).parents('#nav').find('a').removeClass('corrente');      
	  $(this).addClass('corrente');
  })
})

$(document).ready(function(){
  $('#container_abas_consultas').prepend('<ul id="nav"><li><a href="#" class="corrente">Estoque</a></li><li><a href="#">Movimentação</a></li><li><a href="#">Consumo</a></li></ul>')
  
  $('.aba').css({
    display: 'none',
    marginTop: 0,
    borderTopWidth: 0
    })
    
  $('#aba1').css('display', 'block');
  $('a', $('#nav')).click(function() {    
	var i = $('a', $('#nav')).index(this) + 1;      
	  $(this).parents('#container_abas_consultas')
	  .children('.aba:visible').hide();      
	  $('#aba' + i).show();      
	  $(this).parents('#nav').find('a').removeClass('corrente');      
	  $(this).addClass('corrente');
  })
})