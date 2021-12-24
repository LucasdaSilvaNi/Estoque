/***********************
Animação do Menu Superior
***********************/
/**
 * @author Alexander Farkas
 * v. 1.02
 */
(function($) {
	$.extend($.fx.step,{
	    backgroundPosition: function(fx) {
            if (fx.state === 0 && typeof fx.end == 'string') {
                var start = $.curCSS(fx.elem,'backgroundPosition');
                start = toArray(start);
                fx.start = [start[0],start[2]];
                var end = toArray(fx.end);
                fx.end = [end[0],end[2]];
                fx.unit = [end[1],end[3]];
			}
            var nowPosX = [];
            nowPosX[0] = ((fx.end[0] - fx.start[0]) * fx.pos) + fx.start[0] + fx.unit[0];
            nowPosX[1] = ((fx.end[1] - fx.start[1]) * fx.pos) + fx.start[1] + fx.unit[1];
            fx.elem.style.backgroundPosition = nowPosX[0]+' '+nowPosX[1];

           function toArray(strg){
               strg = strg.replace(/left|top/g,'0px');
               strg = strg.replace(/right|bottom/g,'100%');
               strg = strg.replace(/([0-9\.]+)(\s|\)|$)/g,"$1px$2");
               var res = strg.match(/(-?[0-9\.]+)(px|\%|em|pt)\s(-?[0-9\.]+)(px|\%|em|pt)/);
               return [parseFloat(res[1],10),res[2],parseFloat(res[3],10),res[4]];
           }
        }
	});
})(jQuery);

$(function(){	    
	    $('#jqueryAnimated a')
		.css( {backgroundPosition: "0 0"} )
		.mouseover(function(){
			$(this).stop().animate({backgroundPosition:"(0 -64px)"}, {duration:300})
		})
		.mouseout(function(){
			$(this).stop().animate({backgroundPosition:"(0 0)"}, {duration:300})
		})
		
		$('#jqueryAnimated a.selected')
		.css( {backgroundPosition: "0 30px"} )
		.mouseover(function(){
			$(this).stop().animate({backgroundPosition:"(0 30px)"})
		})
		.mouseout(function(){
			$(this).stop().animate({backgroundPosition:"(0 30px)"})
		})
    });

// Contraste 
$(document).ready(function(){			
   $('.contraste').click(function() {
		if ( getActiveStyleSheet() == 'pattern' ){
			setActiveStyleSheet('modify');
			}
		else {
			setActiveStyleSheet('pattern');
			}
	});
});

function setActiveStyleSheet(title) {
  var i, a, main;
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if(a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title")) {
      a.disabled = true;
      if(a.getAttribute("title") == title) a.disabled = false;
    }
  }
}

function getActiveStyleSheet() {
  var i, a;
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if(a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title") && !a.disabled) return a.getAttribute("title");
  }
  return null;
}

function getPreferredStyleSheet() {
  var i, a;
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if(a.getAttribute("rel").indexOf("style") != -1
       && a.getAttribute("rel").indexOf("alt") == -1
       && a.getAttribute("title")
       ) return a.getAttribute("title");
  }
  return null;
}

// Aumenta/Diminui Tamanho da Fonte 
$(document).ready(function(){
  $(".increase").click(function(){
        increaseFont('#global');
    return false;
  });
   $(".decrease").click(function(){  
		decreaseFont('#global');
    return false;
  });
});

function increaseFont(target) {
	
	if ($(target).hasClass('maior1')) {
		$(target).removeClass('maior1');
		$(target).addClass('maior2'); 
	} else  if ($(target).hasClass('maior2')) {
		$(target).removeClass('maior2');
		$(target).addClass('maior3'); 
	} else  if ($(target).hasClass('maior3')) {
		$(target).removeClass('maior3');
		$(target).addClass('maior4'); 
	}
	
	$(target).not('[class]').addClass('maior1');
	
	if ($(target).hasClass('menor1')) {
		$(target).removeClass("menor1"); 
	}
		
	if ($(target).hasClass('menor4')) {
		$(target).removeClass("menor4"); 
		$(target).addClass("menor3"); 
	}  else if ($(target).hasClass('menor3')) {
		$(target).removeClass("menor3"); 
		$(target).addClass("menor2"); 
	}   else if ($(target).hasClass('menor2')) {
		$(target).removeClass("menor2"); 
		$(target).addClass("menor1"); 
	}
	
}

function decreaseFont(target) {
	
	if ($(target).hasClass('menor1')) {
		$(target).removeClass("menor1"); 
		$(target).addClass("menor2"); 
	} 	else if ($(target).hasClass('menor2')) {
			$(target).removeClass("menor2"); 
			$(target).addClass("menor3"); 
	}   else if ($(target).hasClass('menor3')) {
		$(target).removeClass("menor3"); 
		$(target).addClass("menor4"); 
	}
	
	$(target).not('[class]').addClass('menor1');
	
	if ($(target).hasClass('maior1')) {
		$(target).removeClass("maior1"); 
	}

	if ($(target).hasClass('maior4')) {
		$(target).removeClass("maior4"); 
		$(target).addClass("maior3"); 
	}  else if ($(target).hasClass('maior3')) {
		$(target).removeClass("maior3"); 
		$(target).addClass("maior2"); 
	}   else if ($(target).hasClass('maior2')) {
		$(target).removeClass("maior2"); 
		$(target).addClass("maior1"); 
	}	

}

