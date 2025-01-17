﻿using System.Threading;
using System.Threading.Tasks;
using CQRS_Simple.API.Products.Handlers;
using CQRS_Simple.Domain.Products;
using CQRS_Simple.Infrastructure.Dapper;
using CQRS_Simple.Infrastructure.MQ;
using CQRS_Simple.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace CQRS_Simple.API.Products.Commands
{
    public class CreateProductCommandValidate : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidate()
        {
            RuleFor(x => x.Product.Code).Length(10, 256);
        }
    }


    public class CreateProductCommand : IRequest<int>
    {
        public Product Product { get; set; }

        public CreateProductCommand(Product product)
        {
            Product = product;
        }

        public class CreateProductHandle : IRequestHandler<CreateProductCommand, int>
        {
            //private readonly IDapperRepository<Product, int> _dapperRepository;
            private readonly IRepository<Product, int> _repository;
            private readonly RabbitMQClient _mq;

            public CreateProductHandle(RabbitMQClient mq, IRepository<Product, int> repository)
            {
                _mq = mq;
                _repository = repository;
            }

            public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
            {
                _repository.Add(request.Product);
                var result = 1;
                _mq.PushMessage(new RabbitData(typeof(CreateProductCommand), request, result));

                return result;
            }
        }
    }
}