using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eNotaryWebRole.Models
{
    public interface IDataAccessRepository
    {
         string getRole(string username);
         Act create_Act(long act_type_id, string act_name, string act_reason, string act_reason_state, string state, string act_extra_details);
         void update_Act( long id,long act_type_id, string act_name,  string act_reason, string act_reason_state, string state, string act_extra_details);
         void update_SignedAct(long id, long act_type_id, string act_name, string act_reason_signed,  bool act_sent_to_client, bool act_signed, string act_extra_details, string act_reason);
    }
}