using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEHv1.Services.Models;

namespace MEHv1.Services.Validation
{
  public static class ValidatePost
  {

    public static Post Validate(Post post)
    {
      //check/fix formatting
      //make sure all images referenced are present and all present images are referenced
      //make sure Json looks good
			//replace image tags with proper href tags during validation
      return post;
    }

  }
}
