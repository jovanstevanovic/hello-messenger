/**
 *  BaseController.cs
 *  Autor: Nikola Pavlović
 */
using BrzeBoljeJeftinije.Messenger.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrzeBoljeJeftinije.Messenger.UI.Controllers
{
    /**
     * <summary>Osnovna klasa za sve kontrolere, obezbeđuje pravilan commit i rollback u slučaju
     * uspešne ili neuspešne obrade zahteva respektivno</summary>
     *  <remarks>
     *  Verzija: 1.0
     *  </remarks>
     */
    public abstract class BaseController:Controller
    {
        protected readonly IDBProvider dbProvider;
        protected BaseController(IDBProvider dbProvider)
        {
            this.dbProvider = dbProvider;
        }

        /**
         * <summary>Poziva samu akciju kontrolera, hvata greške ako se dese</summary>
         */
        protected override void EndExecute(IAsyncResult asyncResult)
        {
            try
            {
                base.EndExecute(asyncResult);
                dbProvider.CommitIfNecessary();
            }
            catch
            {
                dbProvider.RollbackIfNecessary();
            }
        }

        /**
         * <summary>Greške koje su se desile pri validaciji sadržaja trenutnog HTTP zahteva</summary>
         */
        protected IEnumerable<string> ModelValidationErrors
        {
            get
            {
                return ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage);
            }
        }
    }
}