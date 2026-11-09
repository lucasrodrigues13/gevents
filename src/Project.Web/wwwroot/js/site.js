(function () {
  function marcarInvalidos(raiz) {
    (raiz || document).querySelectorAll(".input-validation-error").forEach(function (campo) {
      campo.classList.add("is-invalid");
      campo.setAttribute("aria-invalid", "true");
    });
  }

  marcarInvalidos(document);

  if (window.jQuery) {
    window.jQuery(document).on("invalid-form.validate", "form", function () {
      marcarInvalidos(this);
    });
  }

  document.addEventListener("submit", function (event) {
    var form = event.target;
    if (!(form instanceof HTMLFormElement)) {
      return;
    }

    if (form.dataset.geLock === "1") {
      event.preventDefault();
      return;
    }

    form.dataset.geLock = "1";
    window.setTimeout(function () {
      form.querySelectorAll('button[type="submit"]').forEach(function (botao) {
        botao.disabled = true;
        botao.setAttribute("aria-busy", "true");
      });
    }, 0);
  });

  document.querySelectorAll("[data-ge-toggle]").forEach(function (campo) {
    var alvo = document.getElementById(campo.getAttribute("data-ge-toggle"));
    if (!alvo) {
      return;
    }

    var atualizar = function () {
      alvo.classList.toggle("d-none", !campo.checked);
    };

    campo.addEventListener("change", atualizar);
    atualizar();
  });

  document.querySelectorAll('[data-ge-abrir="true"]').forEach(function (modal) {
    if (window.bootstrap && window.bootstrap.Modal) {
      window.bootstrap.Modal.getOrCreateInstance(modal).show();
    }
  });
})();
