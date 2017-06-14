function init_slider() {
	var v = $('div.container').width();
	$('#best-products').css({'width':100+'%'});
	
	if(v/4 < 208) count = 3;
	else count = 4;
	console.log(v);
	$("#best-products").jCarouselLite({
		btnNext: "#best-products .next-button",
		btnPrev: "#best-products .prev-button",
		visible:count,
		autoWidth:true
	});
	
	$("#new-products").jCarouselLite({
		btnNext: "#new-products .next-button",
		btnPrev: "#new-products .prev-button",
		visible:count,
		autoWidth:true
	});
}

function sorting() {
	
	alert(1);
}


function sizing() {
	
	alert(2);
}

$(function() {
	init_slider();
	$('.dropdown-toggle').dropdownHover();
	$('.back-to-top').click(function(){
		$(this).parents('body').scrollTo(0, 800, { queue:true });
	});
	
	$(window).resize(function() {
		init_slider(count);
	});
	
	$('.checkboxlist span').click(function(){
		var pos = $(this).offset();
		if($(this).hasClass('checked')) {
			$(this).removeClass('checked');
			$(this).find('input').removeAttr('checked')
		}
		else {
			$(this).addClass('checked');
			$(this).find('input').attr('checked', 'checked')
		}
		//$('#filterForm').submit()
		$('.filter-apply').show().css('top',(pos.top-190));
		
	});
	
	$(".product-photos a[rel^='prettyPhoto']").prettyPhoto;
	
	$('.input-group span.plus').click(function(){
		var count = parseInt($(this).parents('.input-group').find('input[name="NewQuantity"]').val());
		count +=1;
		$(this).parents('.input-group').find('input[name="NewQuantity"]').val(count);
		$(this).parents('form').submit();
	})
	
	$('.input-group span.minus').click(function(){
		var count = parseInt($(this).parents('.input-group').find('input[name="NewQuantity"]').val());
		if(count>1) {
			count = count-1;
			$(this).parents('.input-group').find('input[name="NewQuantity"]').val(count);
			$(this).parents('form').submit();
		}
	});
	$('.order-form input[name="Name"]').keyup(function(){
		$(this).removeClass('error');
	})
	$('.order-form input[name="Phone"]').keyup(function(){
		$(this).removeClass('error');
	})
	$('.order-form input[name="Address"]').keyup(function(){
		$(this).removeClass('error');
	})
	$('.order-form input[name="Email"]').keyup(function(){
		$(this).removeClass('error');
	})
	
	$('input.place-order:not(.final)').click(function(){
		if($('.order-form input[name="Name"]').val()=='') {
			$('.order-form input[name="Name"]').addClass('error');
			return false;
		}
		if($('.order-form input[name="Phone"]').val()==''){
			$('.order-form input[name="Phone"]').addClass('error');
			return false;
		}
		if($('.order-form input[name="Address"]').val()==''){
			$('.order-form input[name="Address"]').addClass('error');
			return false;
		}
		if($('.order-form input[name="Email"]').val()==''){
			$('.order-form input[name="Email"]').addClass('error');
			return false;
		}
		// Включаем индикатор
		$('.overlay').show();
		$('.order-form form').submit()
		
	})
	
	if($("#OrderSuccess").length) $("#OrderSuccess").modal('toggle');
	
	$('.input-group span.delete').click(function(){
		if(confirm('Delete this item?')) {
			var id = parseInt($(this).parents('.input-group').find('input[name="UpdateItem"]').val());
			$.ajax({
			  type: "POST",
			  url: '/basket',
			  data: {DeleteItem:id},
			  success: function(){
				  window.location.reload();
			  },
			  dataType: 'html'
			});
		}
		
	})
	
	$( "#slider-range" ).slider({
		range: true,
		min: parseInt($('input[name="priceMin"]').val()),
		max: parseInt($('input[name="priceMax"]').val()),
		values: [ parseInt($('input[name="priceMin"]').val()), parseInt($('input[name="priceMax"]').val()) ],
		slide: function( event, ui ) {
			$('input[name="priceMin"]').val(ui.values[ 0 ]);
			$('input[name="priceMax"]').val(ui.values[ 1 ]);
		},
		stop: function( event, ui ) {
			var pos = $('#slider-range').offset();
			//$('#filterForm').submit()
			$('.filter-apply').show().css('top',(pos.top-190));
		}
	});
	
	
	
	$('select[name="pageSize"]').change(function(){
		$('.sort_block form').submit()
	});
	
	$('.clearall').click(function(){
		window.location = window.location.pathname;
	});
	
	
	 $(".auth-types").each(function() {
	 	$(this).delegate($(this), "click", function(ev) {
	 		$(".overlayUser").show();
	        ev.preventDefault();
	        if ($(this).data("form")) {
	        	var id = "#"+$(this).data("form");
	        	
	            $(id).show();
	            $(id).parent().find("form.authForm:not("+id+")").each(function() {
	 				$(this).hide();
	 				
	 			});
	            return false;
	        }
		});
	});
	
	$("form.authForm h2").each(function() {
	 	$(this).delegate($(this), "click", function(ev) {
	 		$(this).parent().parent().hide();
		});
	});
	$(".auth-types.problem").each(function() {
	 	$(this).trigger("click");
	});
	
	$('#map_button_yes').click(function(){
		$('#AddressForm').submit();
	});
	
	$('.place-order.final').click(function(){
		$('#deliveryComments').val($('#comments').val());
		$('#OrderForm').submit();
	});
	
	$( "#datepicker" ).datepicker();
	$( "#datepicker1" ).datepicker();
	/*
	var params = {
		changedEl: ".sort_panel select",
		visRows: 5,
		scrollArrows: true
	}
	cuSel(params);
	*/
});
